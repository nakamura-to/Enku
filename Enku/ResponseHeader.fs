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
module ResponseHeader =

  let AcceptRanges value = fun (header: HttpResponseHeaders) ->
    header.AcceptRanges.Add(value)

  let Age value = fun (header: HttpResponseHeaders) ->
    header.Age <- Nullable(value)

  let CacheControl value = fun (header: HttpResponseHeaders) ->
    header.CacheControl <- value

  let Connection value = fun (header: HttpResponseHeaders) ->
    header.Connection.Add value

  let ConnectionClose value = fun (header: HttpResponseHeaders) ->
    header.ConnectionClose <- Nullable(value)

  let Date value = fun (header: HttpResponseHeaders) ->
    header.Date <- Nullable(value)

  let ETag value = fun (header: HttpResponseHeaders) ->
    header.ETag <- value

  let Location value = fun (header: HttpResponseHeaders) ->
    header.Location <- value

  let Pragma value = fun (header: HttpResponseHeaders) ->
    header.Pragma.Add(value)
    
  let ProxyAuthenticate value = fun (header: HttpResponseHeaders) ->
    header.ProxyAuthenticate.Add(value)
    
  let RetryAfter value = fun (header: HttpResponseHeaders) ->
    header.RetryAfter <- value

  let Server value = fun (header: HttpResponseHeaders) ->
    header.Server.Add(value)

  let Trailer value = fun (header: HttpResponseHeaders) ->
    header.Trailer.Add(value)

  let TransferEncoding value = fun (header: HttpResponseHeaders) ->
    header.TransferEncoding.Add(value)

  let TransferEncodingChunked value = fun (header: HttpResponseHeaders) ->
    header.TransferEncodingChunked <- Nullable(value)

  let Upgrade value = fun (header: HttpResponseHeaders) ->
    header.Upgrade.Add(value)

  let Vary value = fun (header: HttpResponseHeaders) ->
    header.Vary.Add(value)

  let Via value = fun (header: HttpResponseHeaders) ->
    header.Via.Add(value)

  let WwwAuthenticate value = fun (header: HttpResponseHeaders) ->
    header.WwwAuthenticate.Add(value)