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
open System.Reflection
open System.Net
open System.Net.Http

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Response =

  module Helper =
    
    let make statusCode (Content content) = Response(fun req -> 
      let (Response builder) = content statusCode
      builder req)

  let headers manipulators (Response builder) = Response(fun req ->
    let res = builder req
    List.iter (fun manipulator -> manipulator res) manipulators
    res)

  let reasonPhrase reasonPhrase (Response builder) = Response(fun req ->
    let res = builder req
    res.ReasonPhrase <- reasonPhrase
    res)

  let version version (Response builder) = Response(fun req ->
    let res = builder req
    res.Version <- version
    res)

  let make (statusCode: int) content =
    Helper.make (unbox<HttpStatusCode> (box statusCode)) content

  /// HTTP status 100
  let Continue content = 
    Helper.make HttpStatusCode.Continue content

  /// HTTP status 101
  let SwitchingProtocols content = 
    Helper.make HttpStatusCode.SwitchingProtocols content

  /// HTTP status 200
  let Ok content =
    Helper.make HttpStatusCode.OK content

  /// HTTP status 201
  let Created content =
    Helper.make HttpStatusCode.Created content

  /// HTTP status 202
  let Accepted content =
    Helper.make HttpStatusCode.Accepted content

  /// HTTP status 203
  let NonAuthoritativeInformation content =
    Helper.make HttpStatusCode.NonAuthoritativeInformation content

  /// HTTP status 204
  let NoContent content =
    Helper.make HttpStatusCode.NoContent content

  /// HTTP status 205
  let ResetContent content =
    Helper.make HttpStatusCode.ResetContent content

  /// HTTP status 206
  let PartialContent content =
    Helper.make HttpStatusCode.PartialContent content

  /// HTTP status 300
  let Ambiguous content =
    Helper.make HttpStatusCode.Ambiguous content

  /// HTTP status 300
  let MultipleChoices content =
    Helper.make HttpStatusCode.MultipleChoices content

  /// HTTP status 301
  let Moved content =
    Helper.make HttpStatusCode.Moved content

  /// HTTP status 301
  let MovedPermanently content =
    Helper.make HttpStatusCode.MovedPermanently content

  /// HTTP status 302
  let Found content =
    Helper.make HttpStatusCode.Found content

  /// HTTP status 302
  let Redirect content =
    Helper.make HttpStatusCode.Redirect content

  /// HTTP status 303
  let RedirectMethod content = 
    Helper.make HttpStatusCode.RedirectMethod content

  /// HTTP status 303
  let SeeOther content =
    Helper.make HttpStatusCode.SeeOther content

  /// HTTP status 304
  let NotModified content =
    Helper.make HttpStatusCode.NotModified content

  /// HTTP status 305
  let UseProxy content =
    Helper.make HttpStatusCode.UseProxy content

  /// HTTP status 306
  let Unused content =
    Helper.make HttpStatusCode.Unused content

  /// HTTP status 307
  let RedirectKeepVerb content =
    Helper.make HttpStatusCode.RedirectKeepVerb content

  /// HTTP status 307
  let TemporaryRedirect content =
    Helper.make HttpStatusCode.TemporaryRedirect content

  /// HTTP status 400
  let BadRequest content =
    Helper.make HttpStatusCode.BadRequest content

  /// HTTP status 401
  let Unauthorized content =
    Helper.make HttpStatusCode.Unauthorized content

  /// HTTP status 402
  let PaymentRequired content =
    Helper.make HttpStatusCode.PaymentRequired content

  /// HTTP status 403
  let Forbidden content =
    Helper.make HttpStatusCode.Forbidden content

  /// HTTP status 404
  let NotFound content = 
    Helper.make HttpStatusCode.NotFound content

  /// HTTP status 405
  let MethodNotAllowed content =
    Helper.make HttpStatusCode.MethodNotAllowed content

  /// HTTP status 406
  let NotAcceptable content =
    Helper.make HttpStatusCode.NotAcceptable content

  /// HTTP status 407
  let ProxyAuthenticationRequired content = 
    Helper.make HttpStatusCode.ProxyAuthenticationRequired content

  /// HTTP status 408
  let RequestTimeout content = 
    Helper.make HttpStatusCode.RequestTimeout content

  /// HTTP status 409
  let Conflict content =
    Helper.make HttpStatusCode.Conflict content

  /// HTTP status 410
  let Gone content =
    Helper.make HttpStatusCode.Gone content

  /// HTTP status 411
  let LengthRequired content =
    Helper.make HttpStatusCode.LengthRequired content

  /// HTTP status 412
  let PreconditionFailed content =
    Helper.make HttpStatusCode.PreconditionFailed content

  /// HTTP status 413
  let RequestEntityTooLarge content =
    Helper.make HttpStatusCode.RequestEntityTooLarge content

  /// HTTP status 414
  let RequestUriTooLong content =
    Helper.make HttpStatusCode.RequestUriTooLong content

  /// HTTP status 415
  let UnsupportedMediaType content = 
    Helper.make HttpStatusCode.UnsupportedMediaType content

  /// HTTP status 416
  let RequestedRangeNotSatisfiable content = 
    Helper.make HttpStatusCode.RequestedRangeNotSatisfiable content

  /// HTTP status 417
  let ExpectationFailed content =
    Helper.make HttpStatusCode.ExpectationFailed content

  /// HTTP status 426
  let UpgradeRequired content =
    Helper.make (unbox<HttpStatusCode> (box 426)) content

  /// HTTP status 500
  let InternalServerError content =
    Helper.make HttpStatusCode.InternalServerError content

  /// HTTP status 501
  let NotImplemented content =
    Helper.make HttpStatusCode.NotImplemented content

  /// HTTP status 502
  let BadGateway content =
    Helper.make HttpStatusCode.BadGateway content

  /// HTTP status 503
  let ServiceUnavailable content = 
    Helper.make HttpStatusCode.ServiceUnavailable content

  /// HTTP status 504
  let GatewayTimeout content = 
    Helper.make HttpStatusCode.GatewayTimeout content

  /// HTTP status 505
  let HttpVersionNotSupported content = 
    Helper.make HttpStatusCode.HttpVersionNotSupported content
