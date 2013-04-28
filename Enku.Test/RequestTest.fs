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
open System.IO
open System.Net.Http
open System.Web.Http.Hosting
open System.Web.Http.Routing
open System.Text
open Enku

module RequestTest = 

  [<Test>]
  let ``Request.asyncReadAsString should get a string value``() =
    use req = new HttpRequestMessage()
    req.Content <- new StringContent("hoge")
    let req = Request req
    let value = 
      Request.asyncReadAsString req
      |> Async.RunSynchronously
    value |> isEqualTo "hoge"

  [<Test>]
  let ``Request.asyncReadAsStream should get a stream value``() =
    use req = new HttpRequestMessage()
    req.Content <- new StringContent("hoge")
    let req = Request req
    let stream = 
      Request.asyncReadAsStream req
      |> Async.RunSynchronously
    use reader = new StreamReader(stream)
    reader.ReadToEnd() |> isEqualTo "hoge"

  [<Test>]
  let ``Request.asyncReadAsBytes should get a byte array``() =
    use req = new HttpRequestMessage()
    req.Content <- new StringContent("hoge")
    let req = Request req
    let bytes = 
      Request.asyncReadAsBytes req
      |> Async.RunSynchronously
    Encoding.UTF8.GetString(bytes) |> isEqualTo "hoge"

  type Person = { Name: string; Age: int}

  [<Test>]
  let ``Request.queryString should get a query string value``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "http://hostname/path?name=hoge&child=aaa&child=bbb")
    let req = Request req
    Request.queryString "name" req |> isEqualTo (Some "hoge")
    Request.queryString "child" req |> isEqualTo (Some "aaa")
    Request.queryString "age" req |> isEqualTo None

  [<Test>]
  let ``Request.queryStringMap should get query strings``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "http://hostname/path?name=hoge&age=20&child=aaa&child=bbb")
    let req = Request req
    let qs = Request.queryStringMap req
    qs |> Map.toList |> List.length |> isEqualTo 3
    qs |> Map.tryFind "name" |> isEqualTo (Some "hoge")
    qs |> Map.tryFind "age" |> isEqualTo (Some "20")
    qs |> Map.tryFind "child" |> isEqualTo (Some "aaa")

  [<Test>]
  let ``Request.queryStringsAll should get all query strings``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "http://hostname/path?name=hoge&age=20&child=aaa&child=bbb")
    let req = Request req
    let qs = Request.queryStringsAll req
    qs |> Map.toList |> List.length |> isEqualTo 3
    qs |> Map.tryFind "name" |> isEqualTo (Some ["hoge"])
    qs |> Map.tryFind "age" |> isEqualTo (Some ["20"])
    qs |> Map.tryFind "child" |> isEqualTo (Some ["aaa"; "bbb"])

  [<Test>]
  let ``Request.routeValue should get a string value``() =
    use req = new HttpRequestMessage()
    let route = HttpRoute()
    let routeData = HttpRouteData(route)
    routeData.Values.["version"] <- "1"
    routeData.Values.["lang"] <- "jp"
    req.Properties.[HttpPropertyKeys.HttpRouteDataKey] <- routeData
    let req = Request req
    Request.routeValue "version" req |> isEqualTo (Some "1")
    Request.routeValue "lang" req |> isEqualTo (Some "jp")
    Request.routeValue "id" req |> isEqualTo (None)

  [<Test>]
  let ``Request.routeValueMap should get string values``() =
    use req = new HttpRequestMessage()
    let route = HttpRoute()
    let routeData = HttpRouteData(route)
    routeData.Values.["version"] <- "1"
    routeData.Values.["lang"] <- "jp"
    req.Properties.[HttpPropertyKeys.HttpRouteDataKey] <- routeData
    let req = Request req
    let values = Request.routeValueMap req
    values |> Map.toList |> List.length |> isEqualTo 2
    values |> Map.tryFind "version" |> isEqualTo (Some "1")
    values |> Map.tryFind "lang" |> isEqualTo (Some "jp")

  [<Test>]
  let ``Request.headers should get request headers``() =
    use req = new HttpRequestMessage()
    req.Headers.Host <- "hoge"
    let req = Request req
    req |> Request.headers |> RequestHeaders.Host |> isEqualTo (Some "hoge")
