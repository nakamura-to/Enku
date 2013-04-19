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
module RequestHeader =

  module Helper =

    let ofNullable (nullable: Nullable<_>) =
      if nullable.HasValue then 
        Some nullable.Value 
      else 
        None

    let ofReferece value =
      if value <> null then 
        Some value 
      else 
        None

    let tryPick seq =
      Seq.tryPick (fun v -> Some v) seq


  let Accept (Request req) = 
    Helper.tryPick req.Headers.Accept

  let AcceptAll (Request req) = 
    Seq.toList req.Headers.Accept

  let AcceptCharset (Request req) = 
    Helper.tryPick req.Headers.AcceptCharset

  let AcceptCharsetAll (Request req) = 
    Seq.toList req.Headers.AcceptCharset

  let AcceptEncoding (Request req) = 
    Helper.tryPick req.Headers.AcceptEncoding

  let AcceptEncodingAll (Request req) = 
    Seq.toList req.Headers.AcceptEncoding

  let AcceptLanguage (Request req) = 
    Helper.tryPick req.Headers.AcceptLanguage

  let AcceptLanguageAll (Request req) = 
    Seq.toList req.Headers.AcceptLanguage

  let Authorization (Request req) = 
    Helper.ofReferece req.Headers.Authorization

  let CacheControl (Request req) = 
    Helper.ofReferece req.Headers.CacheControl

  let Connection (Request req) = 
    Helper.tryPick req.Headers.Connection

  let ConnectionAll (Request req) = 
    Seq.toList req.Headers.Connection

  let ConnectionClose (Request req) = 
    Helper.ofNullable req.Headers.ConnectionClose

  let Date (Request req) = 
    Helper.ofNullable req.Headers.Date

  let Expect (Request req) = 
    Helper.tryPick req.Headers.Expect

  let ExpectAll (Request req) = 
    Seq.toList req.Headers.Expect

  let ExpectContinue  (Request req) = 
    Helper.ofNullable req.Headers.ExpectContinue

  let From (Request req) = 
    Helper.ofReferece req.Headers.From

  let Host (Request req) = 
    Helper.ofReferece req.Headers.Host

  let IfMatch (Request req) = 
    Helper.tryPick req.Headers.IfMatch

  let IfMatchAll (Request req) = 
    Seq.toList req.Headers.IfMatch

  let IfModifiedSince (Request req) = 
    Helper.ofNullable req.Headers.IfModifiedSince

  let IfNoneMatch (Request req) = 
    Helper.tryPick req.Headers.IfNoneMatch

  let IfNoneMatchAll (Request req) = 
    Seq.toList req.Headers.IfNoneMatch

  let IfRange (Request req) = 
    Helper.ofReferece req.Headers.IfRange

  let IfUnmodifiedSince (Request req) = 
    Helper.ofNullable req.Headers.IfUnmodifiedSince

  let MaxForwards  (Request req) = 
    Helper.ofNullable req.Headers.MaxForwards

  let Pragma (Request req) = 
    Helper.tryPick req.Headers.Pragma

  let PragmaAll (Request req) = 
    Seq.toList req.Headers.Pragma

  let ProxyAuthorization (Request req) = 
    Helper.ofReferece req.Headers.ProxyAuthorization

  let Range (Request req) = 
    Helper.ofReferece req.Headers.Range

  let Referrer (Request req) = 
    Helper.ofReferece req.Headers.Referrer

  let TE (Request req) = 
    Helper.tryPick req.Headers.TE

  let TEAll (Request req) = 
    Seq.toList req.Headers.TE

  let Trailer (Request req) = 
    Helper.tryPick req.Headers.Trailer

  let TrailerAll (Request req) = 
    Seq.toList req.Headers.Trailer

  let TransferEncoding (Request req) = 
    Helper.tryPick req.Headers.TransferEncoding

  let TransferEncodingAll (Request req) = 
    Seq.toList req.Headers.TransferEncoding

  let TransferEncodingChunked (Request req) = 
    Helper.ofNullable req.Headers.TransferEncodingChunked

  let Upgrade (Request req) = 
    Helper.tryPick req.Headers.Upgrade

  let UpgradeAll (Request req) = 
    Seq.toList req.Headers.Upgrade

  let UserAgent (Request req) = 
    Helper.tryPick req.Headers.UserAgent

  let UserAgentAll (Request req) = 
    Seq.toList req.Headers.UserAgent

  let Via (Request req) = 
    Helper.tryPick req.Headers.Via

  let ViaAll (Request req) = 
    Seq.toList req.Headers.Via

  let Warning (Request req) = 
    Helper.tryPick <| req.Headers.Warning

  let WarningAll (Request req) = 
    Seq.toList req.Headers.Warning

  let Cookie name (Request req) =
    Helper.tryPick <| req.Headers.GetCookies(name)

  let CookieAll (Request req) =
    Seq.toList <| req.Headers.GetCookies()

  let Allow (Request req) =
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.Allow

  let AllowAll (Request req) =
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.Allow

  let ContentDisposition (Request req) =
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentDisposition

  let ContentEncoding (Request req) = 
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.ContentEncoding

  let ContentEncodingAll (Request req) = 
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.ContentEncoding

  let ContentLanguage (Request req) = 
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.ContentLanguage

  let ContentLanguageAll (Request req) = 
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.ContentLanguage

  let ContentLength (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.ContentLength

  let ContentLocation (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentLocation

  let ContentMD5 (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentMD5

  let ContentRange (Request req) = fun ( res: HttpResponseMessage) ->
    if req.Content = null then
      None
    else
      Helper.ofReferece <| res.Content.Headers.ContentRange

  let ContentType (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentType

  let Expires (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.Expires

  let LastModified (Request req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.LastModified
