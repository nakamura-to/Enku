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

namespace Enku

open System.Net.Http

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Advice =

  let around interceptors actions =
    let chain (Around x) (Around y) = Around(fun req body ->
      x req (fun req ->
        y req body))
    let (Around f) = interceptors |> List.reduce chain
    actions |> List.map (fun (predicate: Constraint, action: Action) ->
      let wrapped : Action = fun req -> f req action
      predicate, wrapped)
