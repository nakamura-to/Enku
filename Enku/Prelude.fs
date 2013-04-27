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

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Prelude = 

  type Request = Request of HttpRequestMessage

  type RequestHeaders = RequestHeaders of HttpRequestMessage

  type Response = Response of (HttpRequestMessage -> HttpResponseMessage)

  type ErrorHandler = (Request -> exn -> Response)

  type Constraint = (Request -> bool)

  type Action = (Request -> Async<Response>)

  type ActionDef = Constraint * Action

  type Controller = (Request -> ActionDef list)

  type ControllerDef = string * Controller

  type Router = (unit -> ControllerDef list * ErrorHandler)

  type Around = (Request -> Action -> Async<Response>)
