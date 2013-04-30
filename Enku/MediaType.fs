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

open System.Net.Http.Formatting

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MediaType =

  type T =
    /// content negotiation
    | Neg
    /// media type
    | Name of string
    /// media type formatter
    | Formatter of MediaTypeFormatter

  /// application/json media type
  let Json = Name "application/json"

  /// application/xml media type
  let Xml = Name "application/xml"