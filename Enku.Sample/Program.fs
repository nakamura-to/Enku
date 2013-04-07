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

// normal
route "path/01/{?id}" <| fun req -> 
  [ 
    post
      { return 10 }
    get
      { return {name = "hoge"; age = 20} }
  ]

// action alternatives
route "path/02" <| fun req -> 
  [ 
    (get <|> post)
      { return {name = "foo"; age = 20} }
  ]

// async action
route "path/03" <| fun req -> 
  [ 
    get
      { return async { 
          return {name = "foo"; age = 20} } }
  ]

// do something around an action
route "path/04" <| fun req -> 
  let log req action =
    printfn "before: GET path/04"
    let result = action req
    printfn "after: GET path/04"
    result
  [ 
    get.around(log)
      { printfn "action: GET path/04"
        return {name = "foo"; age = 20} }
  ]

async {
  use server = new HttpSelfHostServer(config)
  do! Async.AwaitTask <| server.OpenAsync().ContinueWith(fun _ -> ())
  use client = new HttpClient(BaseAddress = baseAddress)

  use! response = Async.AwaitTask <| client.GetAsync("path/01/abc")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample01: %A" content 

  use! response = Async.AwaitTask <| client.PostAsync("path/01", new StringContent(""))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample02: %A" content 

  use! response = Async.AwaitTask <| client.GetAsync("path/02")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample03: %A" content

  use! response = Async.AwaitTask <| client.PostAsync("path/02", new StringContent(""))
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample04: %A" content 

  use! response = Async.AwaitTask <| client.GetAsync("path/03")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample05: %A" content

  use! response = Async.AwaitTask <| client.GetAsync("path/04")
  let! content = Async.AwaitTask <| response.Content.ReadAsStringAsync() 
  printfn "sample06: %A" content
}
|> Async.RunSynchronously

Console.ReadKey() |> ignore