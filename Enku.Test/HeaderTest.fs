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
open Enku

module HeaderTest =

  [<Test>]
  let ``<== operator should apply Header.Add``() =
    let f = function
      | Header.Add value -> value
      | _ -> fail ()
    
    f <=> 10 |> isEqualTo 10
    f <| Header.Add 10 |> isEqualTo 10

