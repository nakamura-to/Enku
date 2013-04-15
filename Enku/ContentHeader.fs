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
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Formatting

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ContentHeader =

  let Allow value = fun (headers: HttpContentHeaders) ->
    headers.Allow.Add(value)

  let ContentDisposition value = fun (headers: HttpContentHeaders) ->
    headers.ContentDisposition <- value

  let ContentEncoding value = fun (headers: HttpContentHeaders) ->
    headers.ContentEncoding.Add(value)

  let ContentLanguage value = fun (headers: HttpContentHeaders) ->
    headers.ContentLanguage.Add(value)

  let ContentLength value = fun (headers: HttpContentHeaders) ->
    headers.ContentLength <- Nullable(value)

  let ContentLocation value = fun (headers: HttpContentHeaders) ->
    headers.ContentLocation <- value

  let ContentMD5 value = fun (headers: HttpContentHeaders) ->
    headers.ContentMD5 <- value

  let ContentRange  value = fun ( headers: HttpContentHeaders) ->
    headers.ContentRange <- value

  let ContentType value = fun (headers: HttpContentHeaders) ->
    headers.ContentType <- value

  let Expires value = fun (headers: HttpContentHeaders) ->
    headers.Expires <- Nullable(value)

  let LastModified value = fun (headers: HttpContentHeaders) ->
    headers.LastModified  <- Nullable(value)
