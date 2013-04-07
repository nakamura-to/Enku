//----------------------------------------------------------------------------
//
// Copyright (c) 2013, Toshihiro Nakamura.
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the LICENSE.txt file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

namespace Enku

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Web.Http
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher
open Microsoft.FSharp.Reflection

type ActionResult<'T> = 
  /// complete the current action
  | Completion of 'T
  /// skip the current action and go to the next one
  | Skip

type Action<'T> = Action of (HttpRequestMessage -> ActionResult<'T>)

type AroundFun<'T> = HttpRequestMessage -> (HttpRequestMessage -> ActionResult<'T>) -> ActionResult<'T>

type ActionKind<'T> =
  | Single of HttpMethod * AroundFun<'T>
  | Any of AroundFun<'T>
  | Alternative of ActionKind<'T> list

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Action =
  let run (Action(f)) req = f req

type ActionBuilder(kind) = 
  member this.ActionKind = kind
  member this.Return(x) = Action(fun _ -> Completion(box x))
  member this.Zero() =  this.Return()
  member this.Bind(m, f) = Action(fun req -> 
    match Action.run m req with
    | Completion out -> Action.run (f out) req
    | _ -> Skip)
  member this.Delay(f) = this.Bind(this.Return(), f)
  member this.Run(f) = Action(fun req ->
    let run around = around req (fun req -> Action.run f req)
    let rec loop kind =
      match kind with
      | Any around -> run around
      | Single(method_, around) when method_ = req.Method -> run around
      | Single _ -> Skip
      | Alternative(kinds) -> 
        kinds 
        |> List.tryPick (fun kind ->
          match loop kind with
          | Completion result -> Some result
          | _ -> None)
        |> function
        | Some result -> Completion result
        | _ -> Skip
    loop kind)
  member this.around(f) =
    let compose g = fun req action -> g req (fun req -> f req action)
    let rec loop kind =
      match kind with
      | Single(method_, g) -> Single(method_, compose(g))
      | Any(g) -> Any(compose(g))
      | Alternative(kinds) -> 
        Alternative(kinds |> List.map loop)
    let kind = loop this.ActionKind
    ActionBuilder(kind)
  static member (<|>) (x: ActionBuilder, y: ActionBuilder) =
    ActionBuilder(Alternative [x.ActionKind; y.ActionKind])

[<AutoOpen>]
module Directives =

  let defaultAround = fun req f -> f req

  let get = ActionBuilder(Single(HttpMethod.Get, defaultAround))
  let post = ActionBuilder(Single(HttpMethod.Post, defaultAround))
  let put = ActionBuilder(Single(HttpMethod.Put, defaultAround))
  let delete = ActionBuilder(Single(HttpMethod.Delete, defaultAround))
  let head = ActionBuilder(Single(HttpMethod.Head, defaultAround))
  let options = ActionBuilder(Single(HttpMethod.Options, defaultAround))
  let trace = ActionBuilder(Single(HttpMethod.Trace, defaultAround))
  let patch = ActionBuilder(Single(HttpMethod("PATCH"), defaultAround))
  let any = new ActionBuilder(Any defaultAround)

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Routing =

  let internal isAsync (typ: Type) =
    typ.IsGenericType && 
    typ.GetGenericTypeDefinition().FullName = "Microsoft.FSharp.Control.FSharpAsync`1"

  let internal makeHandler controller = 
    let bindAsync src (req: HttpRequestMessage) =
      let srcType = src.GetType().GetGenericArguments().[0]
      let destType = typeof<Async<HttpResponseMessage>>
      let binderType = FSharpType.MakeFunctionType(srcType, destType)
      let binder = FSharpValue.MakeFunction(binderType, fun obj -> 
        box <|
          match obj with
          | :? HttpResponseMessage as res -> async.Return res
          | _ as any -> async.Return <| req.CreateResponse(HttpStatusCode.OK, any))
      let bindMethod = async.GetType().GetMethod("Bind").MakeGenericMethod([|srcType; typeof<HttpResponseMessage>|])
      bindMethod.Invoke(async, [| src; binder |]) :?> Async<HttpResponseMessage>
      
    { new HttpMessageHandler() with
      override this.SendAsync(req, token) = 
        let actionResult = 
          controller req 
          |> List.fold (fun state action -> 
            match state with
            | Skip -> Action.run action req
            | _ -> state) Skip
        let computation =
          match actionResult with
          | Completion result ->
            match box result with
            | :? Async<HttpResponseMessage> as async-> async
            | :? HttpResponseMessage as res -> async.Return res
            | _ as result ->
              if result <> null && isAsync (result.GetType()) then bindAsync result req
              else async.Return <| req.CreateResponse(HttpStatusCode.OK, result)
          | _ -> 
            async { return new HttpResponseMessage(HttpStatusCode.NotFound) }
        Async.StartAsTask(computation = computation, cancellationToken = token) }
  
  let regex = Regex(@"{\?(?<optional>[^}]*)}") 

  let route (config: HttpConfiguration) url controller =
    let defaults = Dictionary<string, obj>()
    let url = regex.Replace(url, fun (m: Match) ->
      let key = m.Groups.["optional"].Value
      if defaults.ContainsKey(key) then
        defaults.Remove(key) |> ignore
      defaults.Add(key, RouteParameter.Optional)
      "{" + key + "}")
    config.Routes.MapHttpRoute(
      name = url,
      routeTemplate = url,
      defaults = defaults,
      constraints = null,
      handler = makeHandler controller) |> ignore

  let skip = Action(fun _ -> Skip)

  let skipIfNonAjax = Action(fun req ->
    match req.Headers.TryGetValues("X-Requested-With") with
    | true, values -> 
      if values |> Seq.exists ((=) "XMLHttpRequest") then
        Completion()
      else
        Skip
    | _ -> Skip)