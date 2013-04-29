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

  let controller (interceptors: Around list) (actionDefs: ActionDef list) =
    if List.isEmpty interceptors then
      actionDefs
    else
      actionDefs 
      |> List.map (fun (constraint_, a) -> constraint_, action interceptors a)

  let router (interceptors: Around list) (controllerDefs: ControllerDef list) =
    if List.isEmpty interceptors then
      controllerDefs
    else
      controllerDefs
      |> List.map (fun (path, c) -> path, fun req -> controller interceptors (c req))
