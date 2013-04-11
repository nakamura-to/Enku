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

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Validation =

  exception ValidationError of string

  let private key_enkuValidators = "enku_validators"

  let private consValidator (req: HttpRequestMessage) validator =
    let functions =
      match req.Properties.TryGetValue key_enkuValidators with
      | true, lazyFunctions -> 
        req.Properties.Remove(key_enkuValidators) |> ignore
        lazyFunctions :?> (unit -> unit) list
      | _ -> []
    req.Properties.Add(key_enkuValidators, validator :: functions)

  let getValidators (req: HttpRequestMessage) =
    match req.Properties.TryGetValue key_enkuValidators with
    | true, lazyFunctions -> lazyFunctions :?> (unit -> unit) list
    | _ -> []

  let runValidator validator =
    try
      validator()
      None
    with
    | ValidationError msg -> Some msg

  /// validates request parameters
  let validate resultHandler req =
    let result = 
      getValidators req
      |> List.rev
      |> List.choose runValidator
    resultHandler result

  let deferValidation (req: HttpRequestMessage) validator name value =
    let lazyFun = lazy (
      match validator name value with
      | Valid ret -> ret
      | Invalid message -> raise <| ValidationError message)
    consValidator req (fun () -> lazyFun.Force() |> ignore)
    lazyFun

type Form = Form of HttpRequestMessage * FormDataCollection with
  member this.Value key (Validator validator) =
    let (Form(req, pairs)) = this
    pairs
    |> Seq.filter (fun (KeyValue(k, value)) -> k = key)
    |> Seq.map (fun (KeyValue(_, value)) -> value)
    |> Seq.toList
    |> Some
    |> Validation.deferValidation req validator key

type QueryString = QueryString of HttpRequestMessage * KeyValuePair<string, string> seq with
  member this.Value key (Validator validator) =
    let (QueryString(req, pairs)) = this
    pairs
    |> Seq.filter (fun (KeyValue(k, value)) -> k = key)
    |> Seq.map (fun (KeyValue(_, value)) -> value)
    |> Seq.toList
    |> Some
    |> Validation.deferValidation req validator key

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
    return Form(req, form) }
  member this.QueryString = 
    let (Request req) = this
    QueryString <| (req, req.GetQueryNameValuePairs())
  /// validates request parameters
  member this.Validate resultHandler =
    let (Request req) = this
    let result = 
      Validation.getValidators req
      |> List.rev
      |> List.choose Validation.runValidator
    resultHandler result

type Response = Response of HttpRequestMessage with
  member this.ok value headers =
    let (Response req) = this
    req.CreateResponse(HttpStatusCode.OK, value)