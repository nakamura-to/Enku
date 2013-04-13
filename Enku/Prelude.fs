﻿//----------------------------------------------------------------------------
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
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

[<AutoOpen>]
module Prelude = 

  type Either<'L, 'R> =
    | Left of 'L
    | Right of 'R

  type Validator<'V, 'R> = Validator of (string -> 'V option -> Either<string, 'R>) with
    static member (>>=) (Validator(x), Validator(y)) = Validator(fun name value ->
      match x name value with
      | Right ret -> y name ret
      | Left msg -> Left msg)

  exception internal ValidationError of string

  type KeyValuePairSeq = KeyValuePairSeq of KeyValuePair<string, string> seq

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
    member this.Eval((KeyValuePairSeq pairs), key, (Validator validator)) =
      let value =
        pairs
        |> Seq.filter (fun (KeyValue(k, value)) -> k = key)
        |> Seq.map (fun (KeyValue(_, value)) -> value)
        |> Seq.toList
      let lazyValue = this.MakeLazyValue(validator, key, Some value)
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
          makeErrorValue (sprintf "%A is not an instance property" expr)
      this.Force(lazyValue)
      lazyValue

  type Request = Request of HttpRequestMessage

  type ActionBody = (Request -> Async<HttpRequestMessage -> HttpResponseMessage>)

  type Action = Action of (Request -> ActionBody -> Either<unit, Async<HttpRequestMessage -> HttpResponseMessage>>)

  type ErrorHandler = (Request -> exn -> Async<HttpRequestMessage -> HttpResponseMessage>)

  type FormatError(message: string, innerException: exn) =
    inherit Exception(message, innerException)