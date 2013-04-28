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

namespace Enku.Test

open NUnit.Framework
open System
open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open System.Text.RegularExpressions
open Enku
open Enku.Validation

module V = Validator

module ValidationTest = 

  [<Test>]
  let ``Validator.head should return a first value``() =
    let (Validator validator) = Validator.head
    match validator "x" (Some [1..3]) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 1
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.head should recognize an empty list as invalid``() =
    let (Validator validator) = V.head
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not found."

  [<Test>]
  let ``Validator.headWith should accept a customized message``() =
    let (Validator validator) = V.headWith "[%s]"
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.int should convert a value to a int value``() =
    let (Validator validator) = V.int
    match validator "x" (Some "10") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 10
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.int should recognize a character as invalid``() =
    let (Validator validator) = V.int
    match validator "x" (Some "a") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not a int."

  [<Test>]
  let ``Validator.intWith should accept a customized error message``() =
    let (Validator validator) = V.intWith "[%s]"
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.maxlength should check string length``() =
    let (Validator validator) = V.maxlength 5
    match validator "x" (Some "abcde") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "abcde"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.maxlength should recognize a greater value as invalid``() =
    let (Validator validator) = V.maxlength 5
    match validator "x" (Some "abcdef") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x can not be greater than 5 characters."

  [<Test>]
  let ``Validator.maxlength should accept a customized error message``() =
    let (Validator validator) = V.maxlengthWith 5 "[%s %d]" 
    match validator "x" (Some "abcdef") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x 5]"

  [<Test>]
  let ``Validator.range should check value range``() =
    let (Validator validator) = V.range 5M 15M
    match validator "x" (Some 5M) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 5M
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.range should recognize a out-of-range value as invalid``() =
    let (Validator validator) = V.range 5M 15M
    match validator "x" (Some 4M) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not in the range 5M through 15M."

  [<Test>]
  let ``Validator.range should accept a customized error message``() =
    let (Validator validator) = V.rangeWith 5M 15M "[%s %A %A]" 
    match validator "x" (Some 4M) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x 5M 15M]"

  [<Test>]
  let ``Validator.required should require a value``() =
    let (Validator validator) = V.required
    match validator "x" (Some "abcde") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "abcde"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.range should recognize none as invalid``() =
    let (Validator validator) = V.required
    match validator "x" None with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is required."

  [<Test>]
  let ``Validator.required should accept a customized error message``() =
    let (Validator validator) = V.requiredWith "[%s]" 
    match validator "x" None with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.string should strigify a value``() =
    let (Validator validator) = V.string
    match validator "x" (Some 123) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "123"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.string should strigify a null value``() =
    let (Validator validator) = V.string
    match validator "x" (Some null) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo ""
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``validator should skip None value``() =
    let (Validator validator) = V.head
    match validator "x" None with
    | Result.Ok r -> r |> isEqualTo None
    | _ -> fail()

  [<Test>]
  let ``validator should be composable``() =
    let (Validator validator) = V.head <+> V.int <+> V.range 10 20
    match validator "x" (Some ["15"; "30"]) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 15
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Context.Eval should eval querystring value``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=10&name=hoge"))
    let qs = Request.queryStringMap req
    let vc = Validation.Context()
    let id = vc.Eval(qs, "id", V.int <+> V.required)
    let name = vc.Eval(qs, "name", V.string <+> V.required)
    match vc.Errors with
    | [] ->
      id.Value |> isEqualTo 10
      name.Value |> isEqualTo "hoge"
    | h :: _ -> 
      failwith h
    |> ignore

  [<Test>]
  let ``Context.Eval should eval querystring value and produce validation error messages``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=foo&name=hoge&age=bar"))
    let qs = Request.queryStringMap req
    let vc = Context()
    let id = vc.Eval(qs, "id", V.int <+> V.required)
    let name = vc.Eval(qs, "name", V.string <+> V.required)
    let age = vc.Eval(qs, "age", V.int <+> V.required)
    match vc.Errors with
    | [] -> failwith "validatioin should be fail"
    | messages -> printfn "%A" messages; List.length messages |> isEqualTo 2
    |> ignore

  type Person = { Name: string; Age: int }

  [<Test>]
  let ``Context.Eval should eval record properties``() =
    let person = { Name = "hoge"; Age = 30 }
    let vc = Context()
    vc.Eval(<@ person.Name @>, V.maxlength 10) |> ignore
    vc.Eval(<@ person.Age @>, V.range 10 40) |> ignore
    match vc.Errors with
    | [] ->
      ()
    | h :: _ -> 
      failwith h
    |> ignore

  [<Test>]
  let ``Context.Eval should eval record properties and produce validation error messages``() =
    let person = { Name = "hoge"; Age = 50 }
    let vc = Context()
    vc.Eval(<@ person.Name @>, V.maxlength 2) |> ignore
    vc.Eval(<@ person.Age @>, V.range 10 40) |> ignore
    match vc.Errors with
    | messages-> 
      List.length messages |> isEqualTo 2
      messages.[0] |> isEqualTo "Name can not be greater than 2 characters."
      messages.[1] |> isEqualTo "Age is not in the range 10 through 40."
    |> ignore