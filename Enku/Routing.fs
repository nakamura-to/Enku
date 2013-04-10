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
open System.Collections
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Web.Http
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher
open Microsoft.FSharp.Reflection

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
//        let readContent = 
//          if req.Content = null then
//            let tcs = TaskCompletionSource<Hashtable>();
//            tcs.SetResult(Hashtable())
//            tcs.Task
//          else
//            let formatters = req.GetConfiguration().Formatters
//            req.Content.ReadAsAsync<Hashtable>(formatters)
        let computation = async {
//          let! formData = Async.AwaitTask readContent
          let actionResult = 
            controller req 
            |> List.fold (fun state action -> 
              match state with
              | Skip -> Action.run req action
              | _ -> state) Skip
        return!
          match actionResult with
          | Completion result ->
            match box result with
            | :? Async<HttpResponseMessage> as async-> async
            | :? HttpResponseMessage as res -> async { return res }
            | _ as result ->
              if result <> null && isAsync (result.GetType()) then bindAsync result req
              else async { return req.CreateResponse(HttpStatusCode.OK, result) }
          | _ -> 
            async { return new HttpResponseMessage(HttpStatusCode.NotFound) }}
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

