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

  module Helper =

    let format fmt defaultFmt = 
      sprintf <| match fmt with Some fmt -> fmt | _ -> defaultFmt

    let head name values fmt = 
      match values with
      | Some values ->
        match values with
        | [] -> Error (format fmt "%s is not found." name)
        | h :: _ -> Ok (Some h)
      | _ ->
        Ok None

    let int name value fmt =
      match value with
      | Some value ->
        match Int32.TryParse (string (box value)) with
        | true, n -> Ok (Some n)
        | _ -> Error (format fmt "%s is not a int." name)
      | _ ->
        Ok None

    let maxlength max name value fmt =
      match value with
      | Some s ->
        if String.length s > max then
          Error (format fmt "%s can not be greater than %d characters." name max)
        else
          Ok (Some s)
      | _ ->
        Ok None

    let range min max name value fmt =
      let makeMessage() =
        format fmt "%s is not in the range %A through %A." name min max
      match value with
      | Some n ->
        if n < min then
          Error (makeMessage())
        elif n > max then
          Error (makeMessage())
        else
          Ok (Some n)
      | _ ->
        Ok None

    let required name value fmt =
      match value with
      | Some value -> 
        Ok (Some value)
      | _ ->
        Error (format fmt "%s is required." name)

    let string _ value =
      match value with
      | Some value ->
        Ok (Some (string (box value)))
      | _ ->
        Ok None


  let head = Validator(fun name values ->
    Helper.head name values None)

  let headWith fmt = Validator(fun name values ->
    Helper.head name values (Some fmt))

  let int = Validator(fun name value ->
    Helper.int name value None)

  let intWith fmt = Validator(fun name value ->
    Helper.int name value (Some fmt))

  let maxlength max = Validator(fun name value ->
    Helper.maxlength max name value None)

  let maxlengthWith max fmt = Validator(fun name value ->
    Helper.maxlength max name value (Some fmt))

  let range min max = Validator(fun name value ->
    Helper.range min max name value None)

  let rangeWith min max fmt = Validator(fun name value ->
    Helper.range min max name value (Some fmt))

  let required = Validator(fun name value ->
    Helper.required name value None)

  let requiredWith fmt = Validator(fun name value ->
    Helper.required name value (Some fmt))

  let string = Validator(fun name value ->
    Helper.string name value)
