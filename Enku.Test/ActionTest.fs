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
    let action = fun req -> async { return Response(fun _ -> resMessage) }
    match Action.run req action get with
    | Some r ->
      Async.RunSynchronously r |> fun (Response builder) -> 
        builder reqMessage |> isEqualTo resMessage
    | _ -> failwith "fail"

    let reqMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let resMessage = new HttpResponseMessage()
    let req = Request reqMessage
    let action = fun req -> async { return Response(fun _ -> resMessage) }
    match Action.run req action post with
    | Some r ->
      Async.RunSynchronously r |> fun (Response builder) -> 
        builder reqMessage |> isEqualTo resMessage
    | _ -> failwith "fail"

  [<Test>]
  let ``the action should be skipped``() =
    let reqMessage =  new HttpRequestMessage(HttpMethod.Get, "person/1")
    let resMessage = new HttpResponseMessage()
    let req = Request reqMessage
    let action = fun req -> async { return Response(fun _ -> resMessage) }
    match Action.run req action post with
    | Some _ -> failwith "fail"
    | _ -> ()

    let reqMessage = new HttpRequestMessage(HttpMethod.Post, "person")
    let resMessage = new HttpResponseMessage()
    let req = Request reqMessage
    let action = fun req -> async { return Response(fun _ -> resMessage) }
    match Action.run req action get with
    | Some _ -> failwith "fail"
    | _ -> ()

