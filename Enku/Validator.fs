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

type Validator<'value, 'result> = Validator of (string -> 'value option -> Result.T<'result option, string>) with
  static member (<+>) (Validator(x), Validator(y)) = Validator(fun name value ->
    match x name value with
    | Result.Ok ret -> y name ret
    | Result.Error msg -> Result.Error msg)

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
        | [] -> Result.Error (format fmt "%s is not found." name)
        | h :: _ -> Result.Ok (Some h)
      | _ ->
        Result.Ok None

    let int name value fmt =
      match value with
      | Some value ->
        match Int32.TryParse (string (box value)) with
        | true, n -> Result.Ok (Some n)
        | _ -> Result.Error (format fmt "%s is not a int." name)
      | _ ->
        Result.Ok None

    let maxlength max name value fmt =
      match value with
      | Some s ->
        if String.length s > max then
          Result.Error (format fmt "%s can not be greater than %d characters." name max)
        else
          Result.Ok (Some s)
      | _ ->
        Result.Ok None

    let range min max name value fmt =
      let makeMessage() =
        format fmt "%s is not in the range %A through %A." name min max
      match value with
      | Some n ->
        if n < min then
          Result.Error (makeMessage())
        elif n > max then
          Result.Error (makeMessage())
        else
          Result.Ok (Some n)
      | _ ->
        Result.Ok None

    let required name value fmt =
      match value with
      | Some value -> 
        Result.Ok (Some value)
      | _ ->
        Result.Error (format fmt "%s is required." name)

    let string _ value =
      match value with
      | Some value ->
        Result.Ok (Some (string (box value)))
      | _ ->
        Result.Ok None


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
