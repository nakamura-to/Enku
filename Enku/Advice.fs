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

  let private chain (Around x) (Around y) = Around(fun req body ->
    x req (fun req ->
      y req body))

  let action interceptors (action: Action) =
    let (Around f) = interceptors |> List.reduce chain
    let wrapped : Action = fun req -> f req action
    wrapped

  let controller interceptors (controller: Controller) =
    let wrapped : Controller = fun req -> 
      controller req
      |> List.map (fun (constraint_, a) ->
        constraint_, action interceptors a)
    wrapped

  let router interceptors (router: Router) =
    let wrapped : Router = fun () ->
      let controllers, errorHandler = router ()
      let controllers =
        controllers
        |> List.map (fun (path, c) ->
          path, controller interceptors c)
      controllers, errorHandler
    wrapped
