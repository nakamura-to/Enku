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
open System.Net.Http.Formatting
open System.Net.Http
open System.Web.Http
open System.Web.Http.SelfHost
open Newtonsoft.Json.Serialization
open Newtonsoft.Json.Linq
open Enku

let baseAddress = new Uri("http://localhost:9090/")
let config = new HttpSelfHostConfiguration(baseAddress)
config.IncludeErrorDetailPolicy <- IncludeErrorDetailPolicy.Always
config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
let route = Routing.route config

type Person = { name: string; age: int}

let log req res operation = async {
  printfn "BEFORE"
  let! result = operation req res
  printfn "AFTER"
  return result }

// normal
route "path/01/{?id}" <| fun _ -> 
  [ 
    post, fun req res -> async {
      return res.ok {name = "post"; age = 20} [] }

    get, fun req res -> async {
      return res.ok {name = "get"; age = 20} [] } 
  ]

// action alternatives
route "path/02" <| fun _ -> 
  [ 
    (get <|> post), fun req res -> async {
      return res.ok {name = "foo"; age = 20} [] } 
  ]

// do something around an operation
route "path/04" <| fun _ -> 
  [ 
    get, Advice.around log <| fun req res -> async {
      printfn "MAIN: GET path/04"
      return res.ok {name = "foo"; age = 20} [] } 
  ]

route "path/05" <| fun _ -> 
  [ 
    post, fun req res -> async {
      let! content = req.AsyncReadAsString()
      return res.ok content [] }
  ]

route "path/06" <| fun _ -> 
  let raiseFirst = function  [] -> () | h :: _ -> failwith h
  [ 
    post, fun req res ->  async {
      let! form = req.AsyncReadAsForm()
      let vc = ValidationContext()
      let aaa = vc.Add(form, "aaa", Validator.head + Validator.required)
      let bbb = vc.Add(form, "bbb", Validator.head + Validator.required)
      let ccc = vc.Add(form, "ccc", Validator.head + Validator.required)
      vc.Eval() |> raiseFirst
      return res.ok (aaa.Value + bbb.Value + ccc.Value) [] }
  ]

route "path/07/{?id}" <| fun _ -> 
    Advice.aroundAll log <|
    [ 
      post, fun req res -> async {
        printfn "MAIN: POST path/07"
        return res.ok {name = "post"; age = 20} [] }

      get, fun req res -> async {
        printfn "MAIN: GET path/07"
        return res.ok {name = "get"; age = 20} [] } 
    ]

async {
  use server = new HttpSelfHostServer(config)
  do! Async.AwaitTask <| server.OpenAsync().ContinueWith(fun _ -> ())
  use client = new HttpClient(BaseAddress = baseAddress)

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
}
|> Async.RunSynchronously

Console.ReadKey() |> ignore