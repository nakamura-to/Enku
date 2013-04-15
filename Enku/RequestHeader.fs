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

  let Accept (Request req) = 
    "Accept", Seq.toList req.Headers.Accept

  let AcceptCharset (Request req) = 
    "Accept-Charset", Seq.toList req.Headers.AcceptCharset

  let AcceptEncoding (Request req) = 
    "Accept-Encoding", Seq.toList req.Headers.AcceptEncoding

  let AcceptLanguage (Request req) = 
    "Accept-Language", Seq.toList req.Headers.AcceptLanguage

  let Authorization (Request req) = 
    "Authorization", ofReferece req.Headers.Authorization

  let CacheControl (Request req) = 
    "Cache-Control", ofReferece req.Headers.CacheControl

  let Connection (Request req) = 
    "Connection", Seq.toList req.Headers.Connection

  let ConnectionClose (Request req) = 
    "Connection Close", ofNullable req.Headers.ConnectionClose

  let Date (Request req) = 
    "Date", ofNullable req.Headers.Date

  let Expect (Request req) = 
    "Expect", Seq.toList req.Headers.Expect 

  let ExpectContinue  (Request req) = 
    "Expect Continue", ofNullable req.Headers.ExpectContinue

  let From (Request req) = 
    "From", ofReferece req.Headers.From

  let Host (Request req) = 
    "Host", ofReferece req.Headers.Host 

  let IfMatch (Request req) = 
    "If-Match", Seq.toList req.Headers.IfMatch 

  let IfModifiedSince (Request req) = 
    "If-Modified-Since", ofNullable req.Headers.IfModifiedSince

  let IfNoneMatch (Request req) = 
    "If-None-Match", Seq.toList req.Headers.IfNoneMatch 

  let IfRange (Request req) = 
    "If-Range", ofReferece req.Headers.IfRange

  let IfUnmodifiedSince (Request req) = 
    "If-Unmodified-Since", ofNullable req.Headers.IfUnmodifiedSince

  let MaxForwards  (Request req) = 
    "Max-Forwards", ofNullable req.Headers.MaxForwards

  let Pragma (Request req) = 
    "Pragma", Seq.toList req.Headers.Pragma

  let ProxyAuthorization (Request req) = 
    "Proxy-Authorization", ofReferece req.Headers.ProxyAuthorization

  let Range (Request req) = 
    "Range", ofReferece req.Headers.Range

  let Referrer (Request req) = 
    "Referer", ofReferece req.Headers.Referrer

  let TE (Request req) = 
    "TE", Seq.toList req.Headers.TE 

  let Trailer (Request req) = 
    "Trailer", Seq.toList req.Headers.Trailer

  let TransferEncoding (Request req) = 
    "Transfer-Encoding", Seq.toList req.Headers.TransferEncoding 

  let TransferEncodingChunked (Request req) = 
    "Transfer-Encoding chunked", ofNullable req.Headers.TransferEncodingChunked

  let Upgrade (Request req) = 
    "Upgrade", Seq.toList req.Headers.Upgrade

  let UserAgent (Request req) = 
    "User-Agent", Seq.toList req.Headers.UserAgent

  let Via (Request req) = 
    "Via", Seq.toList req.Headers.Via

  let Warning (Request req) = 
    "Warning", Seq.toList req.Headers.Warning

  let Cookie name (Request req) =
    "Cookie(name=" + name + ")", Seq.toList <| req.Headers.GetCookies(name)

  let CookieAll (Request req) =
    "Cookie", Seq.toList <| req.Headers.GetCookies()

