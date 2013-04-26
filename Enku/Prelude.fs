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
open System.Collections.Specialized
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers

[<AutoOpen>]
module Prelude = 

  module Map =

    let findHead key (map: Map<_, _ list>) =
      Map.find key map |> List.head

    let tryFindHead key (map: Map<_, _ list>) =
      match Map.tryFind key map with
      | None -> None
      | Some values ->
        match values with
        | [] -> None
        | h :: _ -> Some h

  type Request = Request of HttpRequestMessage

  type Response = Response of (HttpRequestMessage -> HttpResponseMessage)

  type ErrorHandler = (Request -> exn -> Response)

  type Constraint = (Request -> bool)

  type Action = (Request -> Async<Response>)

  type ActionDef = Constraint * Action

  type Controller = (Request -> ActionDef list)

  type ControllerDef = string * Controller

  type Router = (unit -> ControllerDef list * ErrorHandler)

  type Around = (Request -> Action -> Async<Response>)

  type FormatError(message: string, innerException: exn) =
    inherit Exception(message, innerException)

  let private isTargetMethod m = fun (Request req) -> req.Method = m

  let get : Constraint = isTargetMethod HttpMethod.Get
  let post : Constraint = isTargetMethod HttpMethod.Post
  let put : Constraint = isTargetMethod HttpMethod.Put
  let delete : Constraint = isTargetMethod HttpMethod.Delete
  let head : Constraint = isTargetMethod HttpMethod.Head
  let options : Constraint = isTargetMethod HttpMethod.Options
  let trace : Constraint = isTargetMethod HttpMethod.Trace
  let patch : Constraint = isTargetMethod <| HttpMethod "PATCH"
  let any : Constraint = (fun _ -> true)

  let (<|>) (x: Constraint) (y: Constraint) : Constraint = 
    fun req -> x req || y req

  let (<&>) (x: Constraint) (y: Constraint) : Constraint = 
    fun req -> x req && y req
