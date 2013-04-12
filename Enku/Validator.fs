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

  let private _head name values format = 
    match values with
    | Some values ->
      match values with
      | [] -> Invalid (sprintf format name)
      | h :: _ -> Valid (Some h)
    | _ ->
      Valid None

  let head = Validator(fun name values ->
    _head name values "%s is not found")

  let headWith format = Validator(fun name values ->
    _head name values format)

  let private _int name value format =
    match value with
    | Some value ->
      match Int32.TryParse (string (box value)) with
      | true, n -> Valid (Some n)
      | _ -> Invalid (sprintf format name)
    | _ ->
      Valid None

  let int = Validator(fun name value ->
    _int name value "%s is not Int32")

  let intWith format = Validator(fun name value ->
    _int name value format)

  let private _length max name value format =
    match value with
    | Some s ->
      // TODO
      if String.length s >= max then
        Invalid (sprintf format name max)
      else
        Valid (Some s)
    | _ ->
      Valid None

  let length max = Validator(fun name value ->
    _length max name value "%s is out of range (max=%A)")

  let lengthWith max format = Validator(fun name value ->
    _length max name value format)

  let private _range min max name value format =
    match value with
    | Some n ->
      // TODO
      if n <= min then
        Invalid (sprintf format name min max)
      elif n >= max then
        Invalid (sprintf format name min max)
      else
        Valid (Some n)
    | _ ->
      Valid None

  let range min max = Validator(fun name value ->
    _range min max name value "%s is out of range (min=%A, max=%A)")

  let rangeWith min max format = Validator(fun name value ->
    _range min max name value format)

  let _required name value format =
    match value with
    | Some value -> 
      Valid value
    | _ ->
      Invalid (sprintf format name)

  let required = Validator(fun name value ->
    _required name value "%s is required")

  let requiredWith format = Validator(fun name value ->
    _required name value format)

  let string = Validator(fun name value ->
    match value with
    | Some value ->
      Valid (Some (string (box value)))
    | _ ->
      Valid None)
