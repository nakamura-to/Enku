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
module Constraint = 

  let get : Constraint = fun (Request req) -> req.Method = HttpMethod.Get

  let post : Constraint = fun (Request req) -> req.Method = HttpMethod.Post

  let put : Constraint = fun (Request req) -> req.Method = HttpMethod.Put

  let delete : Constraint = fun (Request req) -> req.Method = HttpMethod.Delete

  let head : Constraint = fun (Request req) -> req.Method = HttpMethod.Head

  let options : Constraint = fun (Request req) -> req.Method = HttpMethod.Options

  let trace : Constraint = fun (Request req) -> req.Method = HttpMethod.Trace

  let patch : Constraint = fun (Request req) -> req.Method =  HttpMethod "PATCH"

  let any : Constraint = (fun _ -> true)

[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ConstraintOperators = 

  let (<|>) (x: Constraint) (y: Constraint) : Constraint = 
    fun req -> x req || y req

  let (<&>) (x: Constraint) (y: Constraint) : Constraint = 
    fun req -> x req && y req