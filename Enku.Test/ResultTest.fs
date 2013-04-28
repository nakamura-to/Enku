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
open Enku

module ResultTest =

  [<Test>]
  let ``Ok and Error should contain each value``() =
    let calc x y =
      if y = 0 then Result.Error "illegal"
      else Result.Ok <| x / y
    calc 10 2 |> isEqualTo (Result.Ok 5)
    calc 10 0 |> isEqualTo (Result.Error "illegal")

