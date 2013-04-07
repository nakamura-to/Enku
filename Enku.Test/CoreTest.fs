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
    let action = get { return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Get, "person/1")
    Action.run action req |> isEqualTo (Completion (box 10))

    let action = post { return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Post, "person")
    Action.run action req |> isEqualTo (Completion (box 10))

    let action = any { return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Head, "person")
    Action.run action req |> isEqualTo (Completion (box 10))

  [<Test>]
  let ``the action should be skipped``() =
    let action = get { return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Post, "person")
    Action.run action req |> isEqualTo Skip

    let action = post { return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Get, "person/1")
    Action.run action req |> isEqualTo Skip

    let action = get {
      do! Routing.skip
      return 10 } 
    use req = new HttpRequestMessage(HttpMethod.Get, "person/1")
    Action.run action req |> isEqualTo Skip

  [<Test>]
  let ``the composite action should be completed``() =
    let action = (get <|> post) { return 10 } 
    
    use req = new HttpRequestMessage(HttpMethod.Get, "person/1")
    Action.run action req |> isEqualTo (Completion (box 10))

    use req = new HttpRequestMessage(HttpMethod.Post, "person")
    Action.run action req |> isEqualTo (Completion (box 10))
