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
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

[<AutoOpen>]
module Prelude = 

  type Either<'L, 'R> =
    | Left of 'L
    | Right of 'R

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

  type Validator<'V, 'R> = Validator of (string -> 'V option -> Either<string, 'R>) with
    static member (<+>) (Validator(x), Validator(y)) = Validator(fun name value ->
      match x name value with
      | Right ret -> y name ret
      | Left msg -> Left msg)

  exception internal ValidationError of string

  type ValidationContext() =
    let errorMessages = ResizeArray<string>()
    member this.Message = Seq.toList errorMessages
    member private this.MakeLazyValue(validator, name, value) =
      lazy (
        match validator name value with
        | Right ret -> ret
        | Left message -> raise <| ValidationError message)
    member private this.Force(lazyValue: Lazy<_>) =
        try
          lazyValue.Force () |> ignore
        with
        | ValidationError message -> 
          errorMessages.Add(message)
    member this.Eval((name, value), (Validator validator)) =
      let lazyValue = this.MakeLazyValue(validator, name, value)
      this.Force(lazyValue)
      lazyValue
    member this.Eval(map: Map<_, _>, key, (Validator validator)) =
      let value = Map.tryFind key map
      let lazyValue = this.MakeLazyValue(validator, key, value)
      this.Force(lazyValue)
      lazyValue
    member this.Eval(expr: Expr<'T>, (Validator validator)) =
      let makeErrorValue message =
        lazy(raise <| ValidationError message)
      let lazyValue =
        match expr with
        | PropertyGet(receiver, propInfo, _) ->
          match receiver with
          | Some receiver ->
            match receiver with
            | Value(receiver, _) ->
              let key = propInfo.Name
              let value = propInfo.GetValue(receiver, null) :?> 'T
              this.MakeLazyValue(validator, key, Some value)
            |_ -> 
              makeErrorValue (sprintf "%A is not a Value expression" receiver)
          |_ -> 
            makeErrorValue (sprintf "%A is not an instance property" propInfo.Name)
        | _ -> 
          makeErrorValue (sprintf "%A is not a property" expr)
      this.Force(lazyValue)
      lazyValue

  type Request = Request of HttpRequestMessage

  type Response = Response of (HttpRequestMessage -> HttpResponseMessage)

  type Action = (Request -> Async<Response>)

  type Around = Around of (Request -> Action -> Async<Response>)

  type Constraint = (Request -> bool)

  type ErrorHandler = (Request -> exn -> Response)

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
