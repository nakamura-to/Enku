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
module RequestHeaders =

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


  let Accept (RequestHeaders req) = 
    Helper.tryPick req.Headers.Accept

  let AcceptAll (RequestHeaders req) = 
    Seq.toList req.Headers.Accept

  let AcceptCharset (RequestHeaders req) = 
    Helper.tryPick req.Headers.AcceptCharset

  let AcceptCharsetAll (RequestHeaders req) = 
    Seq.toList req.Headers.AcceptCharset

  let AcceptEncoding (RequestHeaders req) = 
    Helper.tryPick req.Headers.AcceptEncoding

  let AcceptEncodingAll (RequestHeaders req) = 
    Seq.toList req.Headers.AcceptEncoding

  let AcceptLanguage (RequestHeaders req) = 
    Helper.tryPick req.Headers.AcceptLanguage

  let AcceptLanguageAll (RequestHeaders req) = 
    Seq.toList req.Headers.AcceptLanguage

  let Authorization (RequestHeaders req) = 
    Helper.ofReferece req.Headers.Authorization

  let CacheControl (RequestHeaders req) = 
    Helper.ofReferece req.Headers.CacheControl

  let Connection (RequestHeaders req) = 
    Helper.tryPick req.Headers.Connection

  let ConnectionAll (RequestHeaders req) = 
    Seq.toList req.Headers.Connection

  let ConnectionClose (RequestHeaders req) = 
    Helper.ofNullable req.Headers.ConnectionClose

  let Date (RequestHeaders req) = 
    Helper.ofNullable req.Headers.Date

  let Expect (RequestHeaders req) = 
    Helper.tryPick req.Headers.Expect

  let ExpectAll (RequestHeaders req) = 
    Seq.toList req.Headers.Expect

  let ExpectContinue  (RequestHeaders req) = 
    Helper.ofNullable req.Headers.ExpectContinue

  let From (RequestHeaders req) = 
    Helper.ofReferece req.Headers.From

  let Host (RequestHeaders req) = 
    Helper.ofReferece req.Headers.Host

  let IfMatch (RequestHeaders req) = 
    Helper.tryPick req.Headers.IfMatch

  let IfMatchAll (RequestHeaders req) = 
    Seq.toList req.Headers.IfMatch

  let IfModifiedSince (RequestHeaders req) = 
    Helper.ofNullable req.Headers.IfModifiedSince

  let IfNoneMatch (RequestHeaders req) = 
    Helper.tryPick req.Headers.IfNoneMatch

  let IfNoneMatchAll (RequestHeaders req) = 
    Seq.toList req.Headers.IfNoneMatch

  let IfRange (RequestHeaders req) = 
    Helper.ofReferece req.Headers.IfRange

  let IfUnmodifiedSince (RequestHeaders req) = 
    Helper.ofNullable req.Headers.IfUnmodifiedSince

  let MaxForwards  (RequestHeaders req) = 
    Helper.ofNullable req.Headers.MaxForwards

  let Pragma (RequestHeaders req) = 
    Helper.tryPick req.Headers.Pragma

  let PragmaAll (RequestHeaders req) = 
    Seq.toList req.Headers.Pragma

  let ProxyAuthorization (RequestHeaders req) = 
    Helper.ofReferece req.Headers.ProxyAuthorization

  let Range (RequestHeaders req) = 
    Helper.ofReferece req.Headers.Range

  let Referrer (RequestHeaders req) = 
    Helper.ofReferece req.Headers.Referrer

  let TE (RequestHeaders req) = 
    Helper.tryPick req.Headers.TE

  let TEAll (RequestHeaders req) = 
    Seq.toList req.Headers.TE

  let Trailer (RequestHeaders req) = 
    Helper.tryPick req.Headers.Trailer

  let TrailerAll (RequestHeaders req) = 
    Seq.toList req.Headers.Trailer

  let TransferEncoding (RequestHeaders req) = 
    Helper.tryPick req.Headers.TransferEncoding

  let TransferEncodingAll (RequestHeaders req) = 
    Seq.toList req.Headers.TransferEncoding

  let TransferEncodingChunked (RequestHeaders req) = 
    Helper.ofNullable req.Headers.TransferEncodingChunked

  let Upgrade (RequestHeaders req) = 
    Helper.tryPick req.Headers.Upgrade

  let UpgradeAll (RequestHeaders req) = 
    Seq.toList req.Headers.Upgrade

  let UserAgent (RequestHeaders req) = 
    Helper.tryPick req.Headers.UserAgent

  let UserAgentAll (RequestHeaders req) = 
    Seq.toList req.Headers.UserAgent

  let Via (RequestHeaders req) = 
    Helper.tryPick req.Headers.Via

  let ViaAll (RequestHeaders req) = 
    Seq.toList req.Headers.Via

  let Warning (RequestHeaders req) = 
    Helper.tryPick <| req.Headers.Warning

  let WarningAll (RequestHeaders req) = 
    Seq.toList req.Headers.Warning

  let Cookie name (RequestHeaders req) =
    Helper.tryPick <| req.Headers.GetCookies(name)

  let CookieAll (RequestHeaders req) =
    Seq.toList <| req.Headers.GetCookies()

  let Allow (RequestHeaders req) =
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.Allow

  let AllowAll (RequestHeaders req) =
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.Allow

  let ContentDisposition (RequestHeaders req) =
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentDisposition

  let ContentEncoding (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.ContentEncoding

  let ContentEncodingAll (RequestHeaders req) = 
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.ContentEncoding

  let ContentLanguage (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.tryPick <| req.Content.Headers.ContentLanguage

  let ContentLanguageAll (RequestHeaders req) = 
    if req.Content = null then
      []
    else
      Seq.toList <| req.Content.Headers.ContentLanguage

  let ContentLength (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.ContentLength

  let ContentLocation (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentLocation

  let ContentMD5 (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentMD5

  let ContentRange (RequestHeaders req) = fun ( res: HttpResponseMessage) ->
    if req.Content = null then
      None
    else
      Helper.ofReferece <| res.Content.Headers.ContentRange

  let ContentType (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofReferece <| req.Content.Headers.ContentType

  let Expires (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.Expires

  let LastModified (RequestHeaders req) = 
    if req.Content = null then
      None
    else
      Helper.ofNullable <| req.Content.Headers.LastModified
