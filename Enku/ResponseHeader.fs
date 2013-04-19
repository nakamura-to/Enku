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

  let AcceptRanges value = fun (res: HttpResponseMessage) ->
    res.Headers.AcceptRanges.Add(value)

  let Age value = fun (res: HttpResponseMessage) ->
    res.Headers.Age <- Nullable(value)

  let CacheControl value = fun (res: HttpResponseMessage) ->
    res.Headers.CacheControl <- value

  let Connection value = fun (res: HttpResponseMessage) ->
    res.Headers.Connection.Add value

  let ConnectionClose value = fun (res: HttpResponseMessage) ->
    res.Headers.ConnectionClose <- Nullable(value)

  let Date value = fun (res: HttpResponseMessage) ->
    res.Headers.Date <- Nullable(value)

  let ETag value = fun (res: HttpResponseMessage) ->
    res.Headers.ETag <- value

  let Location value = fun (res: HttpResponseMessage) ->
    res.Headers.Location <- value

  let Pragma value = fun (res: HttpResponseMessage) ->
    res.Headers.Pragma.Add(value)
    
  let ProxyAuthenticate value = fun (res: HttpResponseMessage) ->
    res.Headers.ProxyAuthenticate.Add(value)
    
  let RetryAfter value = fun (res: HttpResponseMessage) ->
    res.Headers.RetryAfter <- value

  let Server value = fun (res: HttpResponseMessage) ->
    res.Headers.Server.Add(value)

  let Trailer value = fun (res: HttpResponseMessage) ->
    res.Headers.Trailer.Add(value)

  let TransferEncoding value = fun (res: HttpResponseMessage) ->
    res.Headers.TransferEncoding.Add(value)

  let TransferEncodingChunked value = fun (res: HttpResponseMessage) ->
    res.Headers.TransferEncodingChunked <- Nullable(value)

  let Upgrade value = fun (res: HttpResponseMessage) ->
    res.Headers.Upgrade.Add(value)

  let Vary value = fun (res: HttpResponseMessage) ->
    res.Headers.Vary.Add(value)

  let Via value = fun (res: HttpResponseMessage) ->
    res.Headers.Via.Add(value)

  let WwwAuthenticate value = fun (res: HttpResponseMessage) ->
    res.Headers.WwwAuthenticate.Add(value)

  let SetCookie value = fun (res: HttpResponseMessage) ->
    res.Headers.AddCookies(seq { yield value })

  let Allow value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.Allow.Add(value)

  let ContentDisposition value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentDisposition <- value

  let ContentEncoding value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentEncoding.Add(value)

  let ContentLanguage value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentLanguage.Add(value)

  let ContentLength value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentLength <- Nullable(value)

  let ContentLocation value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentLocation <- value

  let ContentMD5 value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentMD5 <- value

  let ContentRange  value = fun ( res: HttpResponseMessage) ->
    res.Content.Headers.ContentRange <- value

  let ContentType value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.ContentType <- value

  let Expires value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.Expires <- Nullable(value)

  let LastModified value = fun (res: HttpResponseMessage) ->
    res.Content.Headers.LastModified  <- Nullable(value)