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
open System.IO
open System.Net
open System.Net.Http
open System.Web.Http.Hosting
open System.Web.Http.Routing
open System.Web.Http.SelfHost
open System.Text
open Enku

module AdviceTest =

  [<Test>]
  let ``Advice.action should advice interceptors around a action``() =
    let buf = StringBuilder()

    let around1 = fun req inner -> async {
      buf.Append "a" |> ignore
      let! ret = inner req
      buf.Append "b" |> ignore
      return ret }
      
    let around2 = fun req inner -> async {
      buf.Append "c" |> ignore
      let! ret = inner req
      buf.Append "d" |> ignore
      return ret }

    let action = fun req -> async {
      buf.Append "e" |> ignore
      return Response.Ok "" MediaType.Neg}

    let advicedAction = Advice.action [around1; around2] action

    use req = new HttpRequestMessage()
    let req = Request req
    advicedAction req
    |> Async.RunSynchronously
    |> ignore

    buf.ToString() |> isEqualTo "acedb"

  [<Test>]
  let ``Advice.action should return the original action when interceptors is empty``() =
    let buf = StringBuilder()

    let action = fun req -> async {
      buf.Append "e" |> ignore
      return Response.Ok "" MediaType.Neg}

    let advicedAction = Advice.action [] action

    use req = new HttpRequestMessage()
    let req = Request req
    advicedAction req
    |> Async.RunSynchronously
    |> ignore

    buf.ToString() |> isEqualTo "e"

  [<Test>]
  let ``Advice.controller should advice interceptors around all actions``() =
    let buf = StringBuilder()

    let around1 = fun req inner -> async {
      buf.Append "a" |> ignore
      let! ret = inner req
      buf.Append "b" |> ignore
      return ret }
      
    let around2 = fun req inner -> async {
      buf.Append "c" |> ignore
      let! ret = inner req
      buf.Append "d" |> ignore
      return ret }

    let controller = fun req ->
      [
        get, fun req -> async {
          buf.Append "e" |> ignore
          return Response.Ok "" MediaType.Neg}

        put, fun req -> async {
          buf.Append "f" |> ignore
          return Response.Ok "" MediaType.Neg}
      ]

    let advicedController = Advice.controller [around1; around2] controller
    use req = new HttpRequestMessage()
    let req = Request req
    let actionDefs = advicedController req
    actionDefs
    |> List.iter (fun (_, action) ->
      action req
      |> Async.RunSynchronously
      |> ignore)

    buf.ToString() |> isEqualTo "acedbacfdb"

  [<Test>]
  let ``Advice.controller should return the original controller when interceptors is empty``() =
    let buf = StringBuilder()

    let controller = fun req ->
      [
        get, fun req -> async {
          buf.Append "e" |> ignore
          return Response.Ok "" MediaType.Neg}

        put, fun req -> async {
          buf.Append "f" |> ignore
          return Response.Ok "" MediaType.Neg}
      ]

    let advicedController = Advice.controller [] controller
    use req = new HttpRequestMessage()
    let req = Request req
    let actionDefs = advicedController req
    actionDefs
    |> List.iter (fun (_, action) ->
      action req
      |> Async.RunSynchronously
      |> ignore)

    buf.ToString() |> isEqualTo "ef"

  [<Test>]
  let ``Advice.route should advice interceptors around all actions``() =
    let buf = StringBuilder()

    let around1 = fun req inner -> async {
      buf.Append "a" |> ignore
      let! ret = inner req
      buf.Append "b" |> ignore
      return ret }
      
    let around2 = fun req inner -> async {
      buf.Append "c" |> ignore
      let! ret = inner req
      buf.Append "d" |> ignore
      return ret }
    
    let router = fun _ ->
      [
      "hoge", fun req ->
        [
          get, fun req -> async {
            buf.Append "e" |> ignore
            return Response.Ok "" MediaType.Neg}

          put, fun req -> async {
            buf.Append "f" |> ignore
            return Response.Ok "" MediaType.Neg}
        ]
      "foo", fun req ->
        [
          get, fun req -> async {
            buf.Append "g" |> ignore
            return Response.Ok "" MediaType.Neg}
        ]
      ], fun req e -> Response.Ok "" MediaType.Neg

    let advicedRouter = Advice.router [around1; around2] router
    use req = new HttpRequestMessage()
    let req = Request req
    let controllerDefs, _ = advicedRouter ()
    controllerDefs
    |> List.iter (fun (_, controller) ->
      let actionDefs = controller req
      actionDefs
      |> List.iter (fun (_, action) ->
        action req
        |> Async.RunSynchronously
        |> ignore))

    buf.ToString() |> isEqualTo "acedbacfdbacgdb"

  [<Test>]
  let ``Advice.router should return the original router when interceptors is empty``() =
    let buf = StringBuilder()

    let router = fun _ ->
      [
      "hoge", fun req ->
        [
          get, fun req -> async {
            buf.Append "e" |> ignore
            return Response.Ok "" MediaType.Neg}

          put, fun req -> async {
            buf.Append "f" |> ignore
            return Response.Ok "" MediaType.Neg}
        ]
      "foo", fun req ->
        [
          get, fun req -> async {
            buf.Append "g" |> ignore
            return Response.Ok "" MediaType.Neg}
        ]
      ], fun req e -> Response.Ok "" MediaType.Neg

    let advicedRouter = Advice.router [] router
    use req = new HttpRequestMessage()
    let req = Request req
    let controllerDefs, _ = advicedRouter ()
    controllerDefs
    |> List.iter (fun (_, controller) ->
      let actionDefs = controller req
      actionDefs
      |> List.iter (fun (_, action) ->
        action req
        |> Async.RunSynchronously
        |> ignore))

    buf.ToString() |> isEqualTo "efg"