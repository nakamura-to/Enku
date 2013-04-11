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

type ActionResult = 
  | Completion of  Async<HttpResponseMessage>
  | Skip

type Action = Action of (Request -> Response -> ActionResult)

type Behavior =  Async<HttpResponseMessage> -> Action

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Action = 

  let make predicate operation = Action(fun (Request req) (Response res) ->
    if predicate req then
      Completion operation
    else 
      Skip)

  let run req res (Action f) = f req res

[<AutoOpen>]
module ActionDirectives =

  let private isTargetRequest m (req: HttpRequestMessage) = req.Method = m

  let get = Action.make (isTargetRequest HttpMethod.Get)
  let post = Action.make (isTargetRequest HttpMethod.Post)
  let put = Action.make (isTargetRequest HttpMethod.Put)
  let delete = Action.make (isTargetRequest HttpMethod.Delete)
  let head = Action.make (isTargetRequest HttpMethod.Head)
  let options = Action.make (isTargetRequest HttpMethod.Options)
  let trace = Action.make (isTargetRequest HttpMethod.Trace)
  let patch = Action.make (isTargetRequest <| HttpMethod "PATCH")
  let any = Action.make (fun _ -> true)

  let (<|>) (x: Behavior) (y: Behavior) = fun operation ->
    Action(fun req res ->
      match Action.run req res (x operation) with
      | Skip -> Action.run req res (y operation)
      | completion -> completion )