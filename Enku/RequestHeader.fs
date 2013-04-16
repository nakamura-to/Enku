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
    tryPick req.Headers.Accept

  let AcceptAll (Request req) = 
    Seq.toList req.Headers.Accept

  let AcceptCharset (Request req) = 
    tryPick req.Headers.AcceptCharset

  let AcceptCharsetAll (Request req) = 
    Seq.toList req.Headers.AcceptCharset

  let AcceptEncoding (Request req) = 
    tryPick req.Headers.AcceptEncoding

  let AcceptEncodingAll (Request req) = 
    Seq.toList req.Headers.AcceptEncoding

  let AcceptLanguage (Request req) = 
    tryPick req.Headers.AcceptLanguage

  let AcceptLanguageAll (Request req) = 
    Seq.toList req.Headers.AcceptLanguage

  let Authorization (Request req) = 
    ofReferece req.Headers.Authorization

  let CacheControl (Request req) = 
    ofReferece req.Headers.CacheControl

  let Connection (Request req) = 
    tryPick req.Headers.Connection

  let ConnectionAll (Request req) = 
    Seq.toList req.Headers.Connection

  let ConnectionClose (Request req) = 
    ofNullable req.Headers.ConnectionClose

  let Date (Request req) = 
    ofNullable req.Headers.Date

  let Expect (Request req) = 
    tryPick req.Headers.Expect

  let ExpectAll (Request req) = 
    Seq.toList req.Headers.Expect

  let ExpectContinue  (Request req) = 
    ofNullable req.Headers.ExpectContinue

  let From (Request req) = 
    ofReferece req.Headers.From

  let Host (Request req) = 
    ofReferece req.Headers.Host

  let IfMatch (Request req) = 
    tryPick req.Headers.IfMatch

  let IfMatchAll (Request req) = 
    Seq.toList req.Headers.IfMatch

  let IfModifiedSince (Request req) = 
    ofNullable req.Headers.IfModifiedSince

  let IfNoneMatch (Request req) = 
    tryPick req.Headers.IfNoneMatch

  let IfNoneMatchAll (Request req) = 
    Seq.toList req.Headers.IfNoneMatch

  let IfRange (Request req) = 
    ofReferece req.Headers.IfRange

  let IfUnmodifiedSince (Request req) = 
    ofNullable req.Headers.IfUnmodifiedSince

  let MaxForwards  (Request req) = 
    ofNullable req.Headers.MaxForwards

  let Pragma (Request req) = 
    tryPick req.Headers.Pragma

  let PragmaAll (Request req) = 
    Seq.toList req.Headers.Pragma

  let ProxyAuthorization (Request req) = 
    ofReferece req.Headers.ProxyAuthorization

  let Range (Request req) = 
    ofReferece req.Headers.Range

  let Referrer (Request req) = 
    ofReferece req.Headers.Referrer

  let TE (Request req) = 
    tryPick req.Headers.TE

  let TEAll (Request req) = 
    Seq.toList req.Headers.TE

  let Trailer (Request req) = 
    tryPick req.Headers.Trailer

  let TrailerAll (Request req) = 
    Seq.toList req.Headers.Trailer

  let TransferEncoding (Request req) = 
    tryPick req.Headers.TransferEncoding

  let TransferEncodingAll (Request req) = 
    Seq.toList req.Headers.TransferEncoding

  let TransferEncodingChunked (Request req) = 
    ofNullable req.Headers.TransferEncodingChunked

  let Upgrade (Request req) = 
    tryPick req.Headers.Upgrade

  let UpgradeAll (Request req) = 
    Seq.toList req.Headers.Upgrade

  let UserAgent (Request req) = 
    tryPick req.Headers.UserAgent

  let UserAgentAll (Request req) = 
    Seq.toList req.Headers.UserAgent

  let Via (Request req) = 
    tryPick req.Headers.Via

  let ViaAll (Request req) = 
    Seq.toList req.Headers.Via

  let Warning (Request req) = 
    tryPick <| req.Headers.Warning

  let WarningAll (Request req) = 
    Seq.toList req.Headers.Warning

  let Cookie name (Request req) =
    tryPick <| req.Headers.GetCookies(name)

  let CookieAll (Request req) =
    Seq.toList <| req.Headers.GetCookies()

