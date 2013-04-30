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

module Enku.Sample.Program

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

// configuration
let config = new HttpSelfHostConfiguration("http://localhost:9090/")
config.IncludeErrorDetailPolicy <- IncludeErrorDetailPolicy.Always
config.Formatters.JsonFormatter |> setupJsonFormatter

// data type
type Person = { Name: string; Age: int }

// around interceptor
let appendServerHeader req inner = async {
  let! ret = inner req
  return 
    ret 
    |> Response.headers
      [ ResponseHeaders.Server <=> ProductInfoHeaderValue("Enku.Sample", "0.1") ] }

// routing
let route = Routing.route config

// actions
route "path/1/{?id}" <| fun _ ->
  [
    post, fun req -> async {
      return Response.Ok {Name = "post"; Age = 20} MediaType.Json }

    get, fun req -> async {
      return Response.Ok {Name = "get"; Age = 20} MediaType.Json }
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// action alternatives
route "path/2" <| fun _ ->
  [ 
    get <|> post, fun req -> async {
      return Response.Ok {Name = "foo"; Age = 20} MediaType.Json } 
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// read request header
route "path/3" <| fun _ ->
  [
    get, fun req -> async {
      let host = req |> Request.headers |> RequestHeaders.Host
      let host = match host with Some v -> v | _ -> ""
      return Response.Ok host MediaType.Json} 
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// write to response header
route "path/4" <| fun _ -> 
  [
    get, fun req -> async {
      return 
        Response.Ok "" MediaType.Json
        |> Response.headers
          [ ResponseHeaders.Location <=> Uri("http://www.google.com") ] } 
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// read string content
route "path/5" <| fun _ -> 
  [
    post, fun req -> async {
      let! content = Request.asyncReadAsString req
      return Response.Ok content MediaType.Json }
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// read form content
route "path/6" <| fun _ -> 
  [
    post, fun req -> async {
      let! form = Request.asyncReadAsForm req
      match form with
      | Result.Error (h, _) ->
        return Response.BadRequest h MediaType.Neg
      | Result.Ok form ->
        let vc = Validation.Context()
        let aaa = vc.Eval(form, "aaa", V.head <+> V.required)
        let bbb = vc.Eval(form, "bbb", V.head <+> V.required)
        let ccc = vc.Eval(form, "ccc", V.head <+> V.required)
        match vc.Errors with
        | [] -> return Response.Ok (aaa.Value + bbb.Value + ccc.Value) MediaType.Json
        | h :: _ -> return Response.BadRequest h MediaType.Neg }
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// intercept controller
route "path/7/{?id}" <| fun _ -> 
  Advice.aroundAll [appendServerHeader] <|
  [ 
    get, fun req -> async {
      let id = Request.routeValue "id" req
      let id = match id with Some v -> v | _ -> ""
      return 
        Response.Ok {Name = "get"; Age = 20} MediaType.Json 
        |> Response.headers 
            [ ResponseHeaders.Age <=> TimeSpan(12, 13, 14) ] }
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

// validation error
route "path/8" <| fun _ -> 
  let validate = function
    | Result.Error _ ->
      Result.Error "format error."
    | Result.Ok person ->
      let vc = Validation.Context()
      let name = vc.Eval(<@ person.Name @>, V.required)
      let age = vc.Eval(<@ person.Age @>, V.range 15 20 <+> V.required)
      match vc.Errors with
      | [] -> Result.Ok <| { Name = name.Value; Age = age.Value }
      | h :: _ -> Result.Error h
  [ 
    post, fun req -> async {
      let! person = Request.asyncReadAs<Person> req
      match validate person with
      | Result.Ok person -> return Response.Ok person.Name MediaType.Json
      | Result.Error message -> return Response.BadRequest message MediaType.Neg }
  ],
  fun req e -> Response.InternalServerError e MediaType.Neg

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

  use client = new HttpClient(new ClinetHandler(), BaseAddress = config.BaseAddress)

  printfn "GET path/1"
  use! response = Async.AwaitTask <| client.GetAsync("path/1/abc")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo """{"name":"get","age":20}"""

  printfn "POST path/1"
  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/1", "{}")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo """{"name":"post","age":20}"""

  printfn "GET path/2"
  use! response = Async.AwaitTask <| client.GetAsync("path/2")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo """{"name":"foo","age":20}"""

  printfn "POST path/2"
  use! response = Async.AwaitTask <| client.PostAsync("path/2", new StringContent(""))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo """{"name":"foo","age":20}"""

  printfn "GET path/3"
  use! response = Async.AwaitTask <| client.GetAsync("path/3")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync()
  content |> isEqualTo "\"localhost:9090\""

  printfn "GET path/4"
  use! response = Async.AwaitTask <| client.GetAsync("path/4")
  response.Headers.Location |> isEqualTo (Uri("http://www.google.com"))

  printfn "POST path/5"
  use! response = Async.AwaitTask <| client.PostAsync("path/5", new StringContent("echo"))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync()
  content |> isEqualTo "\"echo\""

  printfn "GET path/6"
  let pairs = dict ["aaa", "xxx"; "bbb", "yyy"; "ccc", "zzz"]
  use! response = Async.AwaitTask <| client.PostAsync("path/6", new FormUrlEncodedContent(pairs))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo "\"xxxyyyzzz\""

  printfn "GET path/7"
  use! response = Async.AwaitTask <| client.GetAsync("path/7/abc")
  let server = Seq.head response.Headers.Server
  server.Product.Name |> isEqualTo "Enku.Sample"
  server.Product.Version |> isEqualTo "0.1"
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo """{"name":"get","age":20}"""

  printfn "POST path/8: validation error"
  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/8", {Name = "hoge"; Age = 11})
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo "\"Age is not in the range 15 through 20.\""

  printfn "POST path/8: format error"
  use! response = Async.AwaitTask <| client.PostAsJsonAsync("path/8", "test")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  content |> isEqualTo "\"format error.\""

  printfn "All Done!"
  Console.ReadKey() |> ignore
}
|> Async.RunSynchronously