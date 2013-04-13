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
    let reqMessage = new HttpRequestMessage(HttpMethod.Get, "person/1")
    let resMessage = new HttpResponseMessage()
    let req = Request <| reqMessage
    let body = fun req -> async { return fun _ -> resMessage }
    match Action.run req body get with
    | Right r ->
      reqMessage |> Async.RunSynchronously r |> isEqualTo resMessage
    | Left _ -> failwith "fail"

    let reqMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let resMessage = new HttpResponseMessage()
    let req = Request reqMessage
    let body = fun req -> async { return fun _ -> resMessage }
    match Action.run req body post with
    | Right r ->
      reqMessage |> Async.RunSynchronously r |> isEqualTo resMessage
    | Left _ -> failwith "fail"

  [<Test>]
  let ``the action should be skipped``() =
    let reqMessage =  new HttpRequestMessage(HttpMethod.Get, "person/1")
    let resMessage = new HttpResponseMessage()
    let req = Request <| reqMessage
    let body = fun req -> async { return fun _ -> resMessage }
    match Action.run req body post with
    | Right _ -> failwith "fail"
    | Left _ -> ()

    let reqMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let resMessage = new HttpResponseMessage()
    let req = Request reqMessage
    let body = fun req -> async { return fun _ -> resMessage }
    match Action.run req body get with
    | Right _ -> failwith "fail"
    | Left _ -> ()

