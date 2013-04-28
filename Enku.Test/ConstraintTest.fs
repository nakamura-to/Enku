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
open System.Net
open System.Net.Http
open Enku

module ConstraintTest = 

  [<Test>]
  let ``Constraint should accept Request``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "http://hostname")
    let req = Request req
    get req |> isEqualTo true
    post req |> isEqualTo false
    any req |> isEqualTo true

  [<Test>]
  let ``Constraint should be composable``() =
    use req = new HttpRequestMessage(HttpMethod.Get, "http://hostname")
    let req = Request req
    (post <|> get) req |> isEqualTo true
    (get <|> post) req |> isEqualTo true
    (put <|> post) req |> isEqualTo false
    (get <&> (fun req -> 
      let uri = Request.requestUri req
      uri.Host = "hostname")) req |> isEqualTo true
    ((fun req -> 
      let uri = Request.requestUri req
      uri.Host = "hostname") <&> get) req |> isEqualTo true
    (get <&> (fun req -> 
      let uri = Request.requestUri req
      uri.Host = "hoge")) req |> isEqualTo false
