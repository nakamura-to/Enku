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

module V = Validator

let setupJsonFormatter (formatter: JsonMediaTypeFormatter) =
  formatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()

let baseAddress = new Uri("http://localhost:9090/")
let config = new HttpSelfHostConfiguration(baseAddress)
config.IncludeErrorDetailPolicy <- IncludeErrorDetailPolicy.Always
config.Formatters.JsonFormatter |> setupJsonFormatter
let route = Routing.route config

type Person = { Name: string; Age: int }

let log = Around(fun req body -> async {
  printfn "BEFORE1"
  try
    return! body req
  finally
    printfn "AFTER1" })

let log2 = Around(fun req body -> async {
  printfn "BEFORE2"
  try
    return! body req
  finally
    printfn "AFTER2" })

let handleError = fun req e ->
  Response.InternalServerError e

// normal
route "path/01/{?id}" <| fun _ ->
  [ 
    post, fun req -> async {
      return Response.Ok {Name = "post"; Age = 20} }

    get, fun req -> async {
      return Response.Ok {Name = "get"; Age = 20} } 
  ], 
  fun req e -> Response.InternalServerError e

// action alternatives
route "path/02" <| fun _ -> 
  [ 
    get <|> post, fun req -> async {
      return Response.Ok {Name = "foo"; Age = 20} } 
  ],
  handleError

// do something around an operation
route "path/04" <| fun _ -> 
  [ 
    get, fun req -> async {
      printfn "MAIN: GET path/04"
      return Response.Ok {Name = "foo"; Age = 20} } 
  ],
  handleError

route "path/05" <| fun _ -> 
  [ 
    post, fun req -> async {
      let! content = Request.asyncReadAsString req
      return Response.Ok content }
  ],
  handleError

route "path/06" <| fun _ -> 
  let raiseFirst = function  [] -> () | h :: _ -> failwith h
  Advice.around [log] <|
  [ 
    post, fun req ->  async {
      printfn "MAIN: POST path/06"
      let! form = Request.asyncReadAsForm req
      let vc = ValidationContext()
      let aaa = vc.Eval(form, "aaa", V.head <+> V.required)
      let bbb = vc.Eval(form, "bbb", V.head <+> V.required)
      let ccc = vc.Eval(form, "ccc", V.head <+> V.required)
      vc.Errors |> raiseFirst
      return Response.Ok (aaa.Value + bbb.Value + ccc.Value) }
  ],
  handleError

route "path/07/{?id}" <| fun _ -> 
  Advice.around [log ; log2] <|
  [ 
    post, fun req -> async {
      let id = Request.getRouteValue "id" req
      let id = match id with Some v -> v | _ -> ""
      printfn "MAIN: POST path/07, %s" id
      return Response.Ok {Name = "post"; Age = 20} }

    get, fun req -> async {
      let id = Request.getRouteValue "id" req
      let id = match id with Some v -> v | _ -> ""
      printfn "MAIN: GET path/07, id=%s" id
      return 
        Response.Ok {Name = "get"; Age = 20}
        |> Response.appendHeaders 
           [ ResponseHeader.Age <| TimeSpan(12, 13, 14)
             ResponseHeader.ContentType <| Headers.MediaTypeHeaderValue("text/plain")] }
  ],
  handleError

route "path/08" <| fun _ -> 
  [ 
    post, fun req -> 
      let validate = function
        | Ok person ->
          let vc = ValidationContext()
          let name = vc.Eval(<@ person.Name @>, V.required)
          let age = vc.Eval(<@ person.Age @>, V.range 15 20 <+> V.required)
          match vc.Errors with
          | [] -> { Name = name.Value; Age = age.Value }
          | h :: _ ->  Response.BadRequest(h) |> Routing.exit
        | Error (head, _) -> Response.BadRequest head |> Routing.exit
      async {
        let! person = Request.asyncTryReadAs<Person> req
        let person = validate person
        return Response.Ok person.Name }
  ],
  handleError

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