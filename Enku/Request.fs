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
open System.Net.Http
open System.Net.Http.Formatting

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Request =

  exception internal ValidationError of string

  /// skips the current action
  let skip = Action(fun _ -> Skip)

  /// skips the current action if the request is not with XMLHttpRequest
  let skipIfNonAjax = Action(fun req ->
    match req.Headers.TryGetValues("X-Requested-With") with
    | true, values -> 
      if values |> Seq.exists ((=) "XMLHttpRequest") then
        Completion()
      else
        Skip
    | _ -> Skip)

  let private key_enkuValidators = "enku_validators"

  let private consValidator (req: HttpRequestMessage) validator =
    let functions =
      match req.Properties.TryGetValue key_enkuValidators with
      | true, lazyFunctions -> 
        req.Properties.Remove(key_enkuValidators) |> ignore
        lazyFunctions :?> (unit -> unit) list
      | _ -> []
    req.Properties.Add(key_enkuValidators, validator :: functions)

  let private getValidators (req: HttpRequestMessage) =
    match req.Properties.TryGetValue key_enkuValidators with
    | true, lazyFunctions -> lazyFunctions :?> (unit -> unit) list
    | _ -> []

  let private runValidator validator =
    try
      validator()
      None
    with
    | ValidationError msg -> Some msg

  /// validates request parameters
  let validate resultHandler = Action(fun req -> 
    let result = 
      getValidators req
      |> List.rev
      |> List.choose runValidator
    Completion <| resultHandler result)

  let private deferValidation (req: HttpRequestMessage) validator name value =
    let lazyFun = lazy (
      match validator name value with
      | Valid ret -> ret
      | Invalid message -> raise <| ValidationError message)
    consValidator req (fun () -> lazyFun.Force() |> ignore)
    Completion lazyFun

  /// gets query string values as lazy
  let queryString name (Validator(validator)) = Action(fun req ->
    req.GetQueryNameValuePairs()
    |> Seq.filter (fun (KeyValue(key, value)) -> name = key)
    |> Seq.map (fun (KeyValue(_, value)) -> value)
    |> Seq.toList
    |> Some
    |> deferValidation req validator name)
