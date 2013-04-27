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

  module Helper =
    let chain x y = fun req inner ->
      x req (fun req -> y req inner)

  let action (interceptors: Around list) (action: Action) =
    if List.isEmpty interceptors then
      action
    else
      let wrap inner : Action = fun req ->
        let f = List.reduce Helper.chain interceptors
        f req action
      wrap action

  let controller (interceptors: Around list) (controller: Controller) =
    if List.isEmpty interceptors then
      controller
    else
      let wrap inner : Controller = fun req -> 
        let actionDefs = controller req
        List.map (fun (constraint_, a) -> constraint_, action interceptors a) actionDefs
      wrap controller

  let router (interceptors: Around list) (router: Router) =
    if List.isEmpty interceptors then
      router
    else
      let wrap inner : Router = fun () ->
        let controllerDefs, errorHandler = router ()
        let controllerDefs = List.map (fun (path, c) -> path, controller interceptors c) controllerDefs
        controllerDefs, errorHandler
      wrap router
