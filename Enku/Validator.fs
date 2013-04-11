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

type ValidationResult<'R> =
  | Valid of 'R
  | Invalid of string

type Validator<'V, 'R> = Validator of (string -> 'V option -> ValidationResult<'R>) with
  static member (+) (Validator(x), Validator(y)) = Validator(fun name value ->
    match x name value with
    | Valid ret -> y name ret
    | Invalid msg -> Invalid msg)

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Validator =

  let head = Validator(fun name values ->
    match values with
    | Some values ->
      match values with
      | [] -> Invalid (sprintf "%s is not found" name)
      | h :: _ -> Valid (Some h)
    | _ ->
      Valid None)

  let string = Validator(fun name value ->
    match value with
    | Some value ->
      Valid (Some (string (box value)))
    | _ ->
      Valid None)

  let int = Validator(fun name value ->
    match value with
    | Some value ->
      match Int32.TryParse value with
      | true, n -> Valid (Some n)
      | _ -> Invalid (sprintf "%s is not int" name)
    | _ ->
      Valid None)

  let required = Validator(fun name value ->
    match value with
    | Some value -> 
      Valid value
    | _ ->
      Invalid (sprintf "%s is required" name))
