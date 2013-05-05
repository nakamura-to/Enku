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
open System.IO
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Text

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Content =

  // set HttpContent
  let http value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode)
      res.Content <- value
      res))

  /// make StreamContent
  let stream value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode)
      res.Content <- new StreamContent(value)
      res))

  /// make ByteArrayContent
  let byteArray value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode)
      res.Content <- new ByteArrayContent(value)
      res))

  // make error response
  let error (exn: exn) = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateErrorResponse(statusCode, exn)
      res))

  /// content negotiation
  let negotiation value = Content(fun statusCode ->
    Response(fun req -> 
      req.CreateResponse(statusCode, value)))

  /// application/json
  let json value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode, value, "application/json")
      res))

  /// application/xml
  let xml value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode, value, "application/xml")
      res))

  /// text/html
  let html value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode)
      res.Content <- new StringContent(value, null, "text/html")
      res))

  /// text/plain
  let plain value = Content(fun statusCode ->
    Response(fun req -> 
      let res = req.CreateResponse(statusCode)
      res.Content <- new StringContent(value, null, "text/plain")
      res))