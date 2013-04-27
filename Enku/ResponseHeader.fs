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
module ResponseHeaders =

  open Header

  let AcceptRanges manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.AcceptRanges.Add(value)
    | Remove item -> res.Headers.AcceptRanges.Remove(item) |> ignore
    | Clear -> res.Headers.AcceptRanges.Clear()

  let Age manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Age <- Nullable(value)
    | _ -> res.Headers.Age <- Nullable()

  let CacheControl manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.CacheControl <- value
    | _ ->  res.Headers.CacheControl <- null

  let Connection manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Connection.Add(value)
    | Remove item -> res.Headers.Connection.Remove(item) |> ignore
    | Clear -> res.Headers.Connection.Clear()

  let ConnectionClose manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.ConnectionClose <- Nullable(value)
    | _ -> res.Headers.ConnectionClose <- Nullable()

  let Date manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Date <- Nullable(value)
    | _ -> res.Headers.Date <- Nullable()

  let ETag manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.ETag <- value
    | _ -> res.Headers.ETag <- null

  let Location manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Location <- value
    | _ -> res.Headers.Location <- null

  let Pragma manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Pragma.Add(value)
    | Remove item -> res.Headers.Pragma.Remove(item) |> ignore
    | Clear -> res.Headers.Clear()

  let ProxyAuthenticate manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.ProxyAuthenticate.Add(value)
    | Remove item -> res.Headers.ProxyAuthenticate.Remove(item) |> ignore
    | Clear -> res.Headers.ProxyAuthenticate.Clear()
    
  let RetryAfter manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.RetryAfter <- value
    | _ -> res.Headers.RetryAfter <- null

  let Server manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Server.Add(value)
    | Remove item -> res.Headers.Server.Remove(item) |> ignore
    | Clear -> res.Headers.Server.Clear()

  let Trailer manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Trailer.Add(value)
    | Remove item -> res.Headers.Trailer.Remove(item) |> ignore
    | Clear -> res.Headers.Trailer.Clear()

  let TransferEncoding manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.TransferEncoding.Add(value)
    | Remove item -> res.Headers.TransferEncoding.Remove(item) |> ignore
    | Clear -> res.Headers.TransferEncoding.Clear()

  let TransferEncodingChunked manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.TransferEncodingChunked <- Nullable(value)
    | _ -> res.Headers.TransferEncodingChunked <- Nullable()

  let Upgrade manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Upgrade.Add(value)
    | Remove item -> res.Headers.Upgrade.Remove(item) |> ignore
    | Clear -> res.Headers.Upgrade.Clear()

  let Vary manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Vary.Add(value)
    | Remove item -> res.Headers.Vary.Remove(item) |> ignore
    | Clear -> res.Headers.Vary.Clear()

  let Via manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.Via.Add(value)
    | Remove item -> res.Headers.Via.Remove(item) |> ignore
    | Clear -> res.Headers.Via.Clear()

  let WwwAuthenticate manipulation = fun (res: HttpResponseMessage) ->
    match manipulation with
    | Add value -> res.Headers.WwwAuthenticate.Add(value)
    | Remove item -> res.Headers.WwwAuthenticate.Remove(item) |> ignore
    | Clear -> res.Headers.WwwAuthenticate.Clear()

  let SetCookie manipulation = fun (res: HttpResponseMessage) ->
    // TODO
    match manipulation with
    | Add value -> res.Headers.AddCookies(seq { yield value })
    | Remove item -> res.Headers.Remove("Set-Cookie") |> ignore
    | Clear -> res.Headers.Remove("Set-Cookie") |> ignore

  let Allow manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.Allow.Add(value)
      | Remove item -> res.Content.Headers.Allow.Remove(item) |> ignore
      | Clear -> res.Content.Headers.Allow.Clear()

  let ContentDisposition manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentDisposition <- value
      | _ -> res.Content.Headers.ContentDisposition <- null

  let ContentEncoding manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentEncoding.Add(value)
      | Remove item -> res.Content.Headers.ContentEncoding.Remove(item) |> ignore
      | Clear -> res.Content.Headers.ContentEncoding.Clear()

  let ContentLanguage manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentLanguage.Add(value)
      | Remove item -> res.Content.Headers.ContentLanguage.Remove(item) |> ignore
      | Clear -> res.Content.Headers.ContentLanguage.Clear()

  let ContentLength manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentLength <- Nullable(value)
      | _ -> res.Content.Headers.ContentLength <- Nullable()

  let ContentLocation manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentLocation <- value
      | _ -> res.Content.Headers.ContentLocation <- null

  let ContentMD5 manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentMD5 <- value
      | _ -> res.Content.Headers.ContentMD5 <- null

  let ContentRange manipulation = fun ( res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentRange <- value
      | _ -> res.Content.Headers.ContentRange <- null

  let ContentType manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.ContentType <- value
      | _ -> res.Content.Headers.ContentType <- null

  let Expires manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.Expires <- Nullable(value)
      | _ -> res.Content.Headers.Expires <- Nullable()

  let LastModified manipulation = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      match manipulation with
      | Add value -> res.Content.Headers.LastModified  <- Nullable(value)
      | _ -> res.Content.Headers.LastModified  <- Nullable()

  let ContentType2 value = fun (res: HttpResponseMessage) ->
    if res.Content <> null then
      res.Content.Headers.ContentType <- value

  module ContentType2 =
    let clear = fun (res: HttpResponseMessage) ->
      if res.Content <> null then
        res.Content.Headers.ContentType <- null