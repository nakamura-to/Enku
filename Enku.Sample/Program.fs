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

// This sample application listens to http://localhost:9090/. 
// By default, listening at a particular HTTP address requires administrator privileges. 
// When you run this application, therefore, you may get this error: 
// "HTTP could not register URL http://+:9090/" 
// There are two ways to avoid this error:
// - Run Visual Studio with elevated administrator permissions, or
// - Use Netsh.exe to give your account permissions to reserve the URL.

module Program

open System
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Web.Http
open System.Web.Http.SelfHost
open Newtonsoft.Json.Serialization
open Newtonsoft.Json.Linq
open Enku

module V = Validation.Validator

let setupJsonFormatter (formatter: JsonMediaTypeFormatter) =
  formatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()

let baseAddress = new Uri("http://localhost:9090/")
let config = new HttpSelfHostConfiguration(baseAddress)
config.IncludeErrorDetailPolicy <- IncludeErrorDetailPolicy.Always
config.Formatters.JsonFormatter |> setupJsonFormatter
let route = Routing.route config

type Person = { Name: string; Age: int }

let log req inner = async {
  printfn "BEFORE1"
  try
    return! inner req
  finally
    printfn "AFTER1" }

let log2 req inner = async {
  printfn "BEFORE2"
  try
    return! inner req
  finally
    printfn "AFTER2" }

route "path/" <| (Advice.router [log] <| fun _ ->
  [
    // normal
    "01/{?id}", fun req ->
      [
        post, fun req -> async {
          return Response.Ok {Name = "post"; Age = 20} }

        get, fun req -> async {
          return Response.Ok {Name = "get"; Age = 20} }
      ]

    // action alternatives
    "02", fun _ -> 
      [ 
        get <|> post, fun req -> async {
          return Response.Ok {Name = "foo"; Age = 20} } 
      ]

    //
    "04", fun _ -> 
      [
        get, fun req -> async {
          let host = req |> Request.headers |> RequestHeaders.Host
          let host = match host with Some v -> v | _ -> ""
          return Response.Ok host } 
      ]

    "05", fun _ -> 
      [
        post, fun req -> async {
          let! content = Request.asyncReadAsString req
          return Response.Ok content }
      ]

    "06", Advice.controller [log] <| fun _ -> 
      [
        post, fun req -> async {
          printfn "MAIN: POST path/06"
          let! form = Request.asyncReadAsForm req
          match form with
          | Result.Ok form ->
            let vc = Validation.Context()
            let aaa = vc.Eval(form, "aaa", V.head <+> V.required)
            let bbb = vc.Eval(form, "bbb", V.head <+> V.required)
            let ccc = vc.Eval(form, "ccc", V.head <+> V.required)
            match vc.Errors with
            | [] -> return Response.Ok (aaa.Value + bbb.Value + ccc.Value)
            | h :: _ -> return Response.BadRequest h
          | Result.Error (h, _) ->
            return Response.BadRequest h }
      ]

    "07/{?id}", Advice.controller [log ; log2] <| fun _ -> 
      [ 
        post, fun req -> async {
          let id = Request.routeValue "id" req
          let id = match id with Some v -> v | _ -> ""
          printfn "MAIN: POST path/07, %s" id
          return Response.Ok {Name = "post"; Age = 20} }

        get, fun req -> async {
          let id = Request.routeValue "id" req
          let id = match id with Some v -> v | _ -> ""
          printfn "MAIN: GET path/07, id=%s" id
          return 
            Response.Ok {Name = "get"; Age = 20}
            |> Response.headers 
               [ ResponseHeaders.Age <== TimeSpan(12, 13, 14)
                 ResponseHeaders.ContentType <| Header.Clear ] }
      ]

    "08", fun _ -> 
      [ 
        post, fun req -> 
          let validate = function
            | Result.Ok person ->
              let vc = Validation.Context()
              let name = vc.Eval(<@ person.Name @>, V.required)
              let age = vc.Eval(<@ person.Age @>, V.range 15 20 <+> V.required)
              match vc.Errors with
              | [] -> Result.Ok <| { Name = name.Value; Age = age.Value }
              | h :: _ -> Result.Error <| Response.BadRequest(h)
            | Result.Error (h, _) -> Result.Error <| Response.BadRequest h
          async {
            let! person = Request.asyncReadAs<Person> req
            match validate person with
            | Result.Ok person -> return Response.Ok person.Name
            | Result.Error error -> return error }
      ]
  ], 
  fun req e -> Response.InternalServerError e)

type ClinetHandler() =
  inherit HttpClientHandler()
  override this.SendAsync(req, token) =
    if req <> null then
      match req.Content with
      | :? ObjectContent as content ->
        match content.Formatter with
        | :? JsonMediaTypeFormatter as formatter ->
          setupJsonFormatter formatter
        | _ -> ()
      | _ -> ()
    base.SendAsync(req, token)

async {
  use server = new HttpSelfHostServer(config)
  do! Async.AwaitTask <| server.OpenAsync().ContinueWith(fun _ -> ())

  use client = new HttpClient(new ClinetHandler(), BaseAddress = baseAddress)
  use! response = Async.AwaitTask <| client.GetAsync("path/01/abc")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample01: %A" content 

  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/01", @"{ ""test"": 10}")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample02: %A" content 

  use! response = Async.AwaitTask <| client.GetAsync("path/02")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample03: %A" content

  use! response = Async.AwaitTask <| client.PostAsync("path/02", new StringContent(""))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample04: %A" content 

  use! response = Async.AwaitTask <| client.GetAsync("path/04")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample06: %A" content

  use! response = Async.AwaitTask <| client.PostAsync("path/05", new StringContent("echo"))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample07: %A" content

  let pairs = dict ["aaa", "xxx"; "bbb", "yyy"; "ccc", "zzz"]
  use! response = Async.AwaitTask <| client.PostAsync("path/06", new FormUrlEncodedContent(pairs))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample08: %A" content

  use! response = Async.AwaitTask <| client.GetAsync("path/07/abc")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample09: %A" content 

  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/07", @"{ ""test"": 10}")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample10: %A" content 

  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/08", {Name = "hoge"; Age = 11})
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample11: %A" content 

  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/08", "test")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample12: %A" content

  Console.ReadKey() |> ignore
}
|> Async.RunSynchronously