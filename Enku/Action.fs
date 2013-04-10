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

open System.Net.Http

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
  let run req (Action(f)) = f req

type ActionBuilder(kind) = 
  member this.ActionKind = kind
  member this.Return(x) = Action(fun _ -> Completion(box x))
  member this.ReturnFrom(m: Action<_>) = m
  member this.Zero() =  this.Return()
  member this.Bind(m, f) = Action(fun req -> 
    match Action.run req m with
    | Completion out -> Action.run req (f out)
    | _ -> Skip)
  member this.Delay(f) = this.Bind(this.Return(), f)
  member this.Run(f) = Action(fun req ->
    let run around = around req (fun req -> Action.run req f)
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
module ActionDirectives =

  let defaultAround = fun req f -> f req
  let get = ActionBuilder(Single(HttpMethod.Get, defaultAround))
  let post = ActionBuilder(Single(HttpMethod.Post, defaultAround))
  let put = ActionBuilder(Single(HttpMethod.Put, defaultAround))
  let delete = ActionBuilder(Single(HttpMethod.Delete, defaultAround))
  let head = ActionBuilder(Single(HttpMethod.Head, defaultAround))
  let options = ActionBuilder(Single(HttpMethod.Options, defaultAround))
  let trace = ActionBuilder(Single(HttpMethod.Trace, defaultAround))
  let patch = ActionBuilder(Single(HttpMethod("PATCH"), defaultAround))
  let any = ActionBuilder(Any defaultAround)