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

module V = Validator

module ValidationTest = 

  [<Test>]
  let ``Context.Eval should eval querystring value and produce validation error messages``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=foo&name=hoge&age=bar"))
    let qs = Request.queryStringMap req
    let vc = Validation.Context()
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
    let vc = Validation.Context()
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
    let vc = Validation.Context()
    vc.Eval(<@ person.Name @>, V.maxlength 2) |> ignore
    vc.Eval(<@ person.Age @>, V.range 10 40) |> ignore
    match vc.Errors with
    | messages-> 
      List.length messages |> isEqualTo 2
      messages.[0] |> isEqualTo "Name can not be greater than 2 characters."
      messages.[1] |> isEqualTo "Age is not in the range 10 through 40."
    |> ignore