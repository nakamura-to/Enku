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

module ResponseTest = 

  [<Test>]
  let ``Response should build a HttpResponseMessage instance``() =
    use config = new HttpSelfHostConfiguration("http://hostname")
    use req = new HttpRequestMessage()
    req.Properties.[HttpPropertyKeys.HttpConfigurationKey] <- config
    let (Response builder) = 
      Response.Ok "hoge"
      |> Response.headers 
        [ ResponseHeaders.Location <=> Uri("http://example.com")
          ResponseHeaders.ContentEncoding <=> "utf-8" ]
    let res = builder req
    res.StatusCode |> isEqualTo HttpStatusCode.OK
    res.RequestMessage |> isEqualTo req
    res.Headers.Location |> isEqualTo (Uri("http://example.com"))
    res.Content.Headers.ContentEncoding |> Seq.head |> isEqualTo "utf-8"
