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
    use reqestMessage = new HttpRequestMessage(HttpMethod.Get, "person/1")
    let req = Request reqestMessage
    let res = Response reqestMessage
    let body = fun req res -> async { 
      return new HttpResponseMessage(Content = content) }
    match Action.run req res body get with
    | Right r ->
      let res = Async.RunSynchronously r
      res.Content |> isEqualTo content
    | Left _ -> failwith "fail"

    use reqestMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let req = Request reqestMessage
    let res = Response reqestMessage
    let body = fun req res ->  async { 
      return new HttpResponseMessage(Content = content) }
    match Action.run req res body post with
    | Right r ->
      let res = Async.RunSynchronously r
      res.Content |> isEqualTo content
    | Left _ -> failwith "fail"

  [<Test>]
  let ``the action should be skipped``() =
    use reqestMessage = new HttpRequestMessage(HttpMethod.Get, "person/1")
    let req = Request reqestMessage
    let res = Response reqestMessage
    let body = fun req res -> async { 
      return new HttpResponseMessage(Content = new StringContent("hoge")) }
    match Action.run req res body post with
    | Right _ -> failwith "fail"
    | Left _ -> ()

    use reqestMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let req = Request reqestMessage
    let res = Response reqestMessage
    let body = fun req res -> async { 
      return new HttpResponseMessage(Content = new StringContent("hoge")) }
    match Action.run req res body get with
    | Right _ -> failwith "fail"
    | Left _ -> ()

