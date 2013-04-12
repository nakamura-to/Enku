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
    let validatorWrappers = ResizeArray<unit -> unit>()
    let addLazyValue (lazyValue: Lazy<_>) =
      validatorWrappers.Add((fun () -> lazyValue.Force() |> ignore))
    member private this.MakeLazyValue(validator, name, value) =
      lazy (
        match validator name value with
        | Right ret -> ret
        | Left message -> raise <| ValidationError message)
    member this.Add((KeyValuePairSeq pairs), key, (Validator validator)) =
      let value =
        pairs
        |> Seq.filter (fun (KeyValue(k, value)) -> k = key)
        |> Seq.map (fun (KeyValue(_, value)) -> value)
        |> Seq.toList
      let lazyValue = this.MakeLazyValue(validator, key, Some value)
      addLazyValue lazyValue
      lazyValue
    member this.Add(expr: Expr<'T>, (Validator validator)) =
      let addError message =
        let lazyValue = lazy(raise <| ValidationError message)
        addLazyValue lazyValue
      match expr with
      | PropertyGet(receiver, propInfo, _) ->
        match receiver with
        | Some receiver ->
          match receiver with
          | Value(receiver, _) ->
            let key = propInfo.Name
            let value = propInfo.GetValue(receiver, null) :?> 'T
            let lazyValue = this.MakeLazyValue(validator, key, Some value)
            addLazyValue lazyValue
          |_ -> addError (sprintf "%A is not a Value expression" receiver)
        |_ -> addError (sprintf "%A is not an instance property" propInfo.Name)
      | _ ->
        addError (sprintf "%A is not an instance property" expr)
    member this.Eval() =
      let runValidator validator =
        try
          validator()
          None
        with
        | ValidationError msg -> Some msg
      validatorWrappers
      |> Seq.choose runValidator
      |> Seq.toList

  type Request = Request of HttpRequestMessage with
    member this.AsyncReadAsString() =
      let (Request req) = this
      Async.AwaitTask <| req.Content.ReadAsStringAsync()
    member this.AsyncReadAsStream() =
      let (Request req) = this
      Async.AwaitTask <| req.Content.ReadAsStreamAsync()
    member this.AsyncReadAsBytes() =
      let (Request req) = this
      Async.AwaitTask <| req.Content.ReadAsByteArrayAsync()
    member this.AsyncReadAsForm() = async {
      let (Request req) = this
      let! form = Async.AwaitTask <| req.Content.ReadAsAsync<FormDataCollection>()
      return KeyValuePairSeq(form) }
    member this.GetQueryString() = 
      let (Request req) = this
      KeyValuePairSeq <| req.GetQueryNameValuePairs()

  type Response = Response of HttpRequestMessage with
    member this.OK value headers =
      let (Response req) = this
      req.CreateResponse(HttpStatusCode.OK, value)


  type ActionBody = (Request -> Response -> Async<HttpResponseMessage>)

  type Action = Action of (Request -> Response -> ActionBody -> Either<unit, Async<HttpResponseMessage>>)

  type ActionErrorHandler = (Request -> Response -> exn -> Async<HttpResponseMessage>)
