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
open System.Net.Http
open System.Text.RegularExpressions
open Enku

module ActionTest = 

  [<Test>]
  let ``the action should be completed``() =
    let content = new StringContent("hoge") :> HttpContent
    use req =  new HttpRequestMessage(HttpMethod.Get, "person/1")
    let action, operation = get, fun req res -> async { 
      return new HttpResponseMessage(Content = content) }
    match Action.run (Request req) (Response req) operation action with
    | Completion result ->
      let res = Async.RunSynchronously result
      res.Content |> isEqualTo content
    | Skip -> failwith "fail"

    use req = new HttpRequestMessage(HttpMethod.Post, "person")
    let action, operation = post, fun req res ->  async { 
      return new HttpResponseMessage(Content = content) }
    match Action.run (Request req) (Response req) operation action with
    | Completion result ->
      let res = Async.RunSynchronously result
      res.Content |> isEqualTo content
    | Skip -> failwith "fail"

  [<Test>]
  let ``the action should be skipped``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "person/1")
    let action, operation= post, fun req res -> async { 
      return new HttpResponseMessage(Content = new StringContent("hoge")) }
    match Action.run (Request req) (Response req) operation action with
    | Completion result -> failwith "fail"
    | Skip -> ()

    use req = new HttpRequestMessage(HttpMethod.Post, "person")
    let action, operation = get, fun req res -> async { 
      return new HttpResponseMessage(Content = new StringContent("hoge")) }
    match Action.run (Request req) (Response req) operation action with
    | Completion result -> failwith "fail"
    | Skip -> ()

