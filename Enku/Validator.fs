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

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Validator =

  let private _head name values format = 
    match values with
    | Some values ->
      match values with
      | [] -> Left (sprintf format name)
      | h :: _ -> Right (Some h)
    | _ ->
      Right None

  let head = Validator(fun name values ->
    _head name values "%s is not found")

  let headWith format = Validator(fun name values ->
    _head name values format)

  let private _int name value format =
    match value with
    | Some value ->
      match Int32.TryParse (string (box value)) with
      | true, n -> Right (Some n)
      | _ -> Left (sprintf format name)
    | _ ->
      Right None

  let int = Validator(fun name value ->
    _int name value "%s is not Int32")

  let intWith format = Validator(fun name value ->
    _int name value format)

  let private _length max name value format =
    match value with
    | Some s ->
      // TODO
      if String.length s >= max then
        Left (sprintf format name max)
      else
        Right (Some s)
    | _ ->
      Right None

  let length max = Validator(fun name value ->
    _length max name value "%s is out of range (max=%A)")

  let lengthWith max format = Validator(fun name value ->
    _length max name value format)

  let private _range min max name value format =
    match value with
    | Some n ->
      // TODO
      if n <= min then
        Left (sprintf format name min max)
      elif n >= max then
        Left (sprintf format name min max)
      else
        Right (Some n)
    | _ ->
      Right None

  let range min max = Validator(fun name value ->
    _range min max name value "%s is out of range (min=%A, max=%A)")

  let rangeWith min max format = Validator(fun name value ->
    _range min max name value format)

  let _required name value format =
    match value with
    | Some value -> 
      Right value
    | _ ->
      Left (sprintf format name)

  let required = Validator(fun name value ->
    _required name value "%s is required")

  let requiredWith format = Validator(fun name value ->
    _required name value format)

  let string = Validator(fun name value ->
    match value with
    | Some value ->
      Right (Some (string (box value)))
    | _ ->
      Right None)
