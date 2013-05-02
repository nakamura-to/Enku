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

open System
open System.Collections.Specialized
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Validation =

  type Context() =
    let errors = ResizeArray<string>()
    member this.Errors = Seq.toList errors
    member private this.Run(Validator validator, name, value) =
      match validator name value with
      | Result.Ok ret -> ret
      | Result.Error msg ->
        errors.Add(msg)
        None
    member this.Eval(value, name, validator) =
      this.Run(validator, name, value)
    member this.Eval(map: Map<_, _>, key, validator) =
      let value = Map.tryFind key map
      this.Run(validator, key, value)
    member this.Eval(expr: Expr<'T>, validator) =
      match expr with
      | PropertyGet(receiver, propInfo, _) ->
        match receiver with
        | Some receiver ->
          match receiver with
          | Value(receiver, _) ->
            let name = propInfo.Name
            let value = propInfo.GetValue(receiver, null) :?> 'T
            this.Run(validator, name, Some value)
          |_ -> 
            errors.Add(sprintf "%A is not a Value expression" receiver)
            None
        |_ -> 
          errors.Add(sprintf "%A is not an instance property" propInfo.Name)
          None
      | _ -> 
        errors.Add(sprintf "%A is not a property" expr)
        None