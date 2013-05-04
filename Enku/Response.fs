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
    
    let make statusCode f = Response(fun req ->
      let (Response builder) = f statusCode
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

  let make (statusCode: int) value =
    Helper.make (unbox<HttpStatusCode> (box statusCode)) value

  /// HTTP status 100
  let Continue value = 
    Helper.make HttpStatusCode.Continue value

  /// HTTP status 101
  let SwitchingProtocols value = 
    Helper.make HttpStatusCode.SwitchingProtocols value

  /// HTTP status 200
  let Ok value =
    Helper.make HttpStatusCode.OK value

  /// HTTP status 201
  let Created value =
    Helper.make HttpStatusCode.Created value

  /// HTTP status 202
  let Accepted value =
    Helper.make HttpStatusCode.Accepted value

  /// HTTP status 203
  let NonAuthoritativeInformation value =
    Helper.make HttpStatusCode.NonAuthoritativeInformation value

  /// HTTP status 204
  let NoContent value =
    Helper.make HttpStatusCode.NoContent value

  /// HTTP status 205
  let ResetContent value =
    Helper.make HttpStatusCode.ResetContent value

  /// HTTP status 206
  let PartialContent value =
    Helper.make HttpStatusCode.PartialContent value

  /// HTTP status 300
  let Ambiguous value =
    Helper.make HttpStatusCode.Ambiguous value

  /// HTTP status 300
  let MultipleChoices value =
    Helper.make HttpStatusCode.MultipleChoices value

  /// HTTP status 301
  let Moved value =
    Helper.make HttpStatusCode.Moved value

  /// HTTP status 301
  let MovedPermanently value =
    Helper.make HttpStatusCode.MovedPermanently value

  /// HTTP status 302
  let Found value =
    Helper.make HttpStatusCode.Found value

  /// HTTP status 302
  let Redirect value =
    Helper.make HttpStatusCode.Redirect value

  /// HTTP status 303
  let RedirectMethod value = 
    Helper.make HttpStatusCode.RedirectMethod value

  /// HTTP status 303
  let SeeOther value =
    Helper.make HttpStatusCode.SeeOther value

  /// HTTP status 304
  let NotModified value =
    Helper.make HttpStatusCode.NotModified value

  /// HTTP status 305
  let UseProxy value =
    Helper.make HttpStatusCode.UseProxy value

  /// HTTP status 306
  let Unused value =
    Helper.make HttpStatusCode.Unused value

  /// HTTP status 307
  let RedirectKeepVerb value =
    Helper.make HttpStatusCode.RedirectKeepVerb value

  /// HTTP status 307
  let TemporaryRedirect value =
    Helper.make HttpStatusCode.TemporaryRedirect value

  /// HTTP status 400
  let BadRequest value =
    Helper.make HttpStatusCode.BadRequest value

  /// HTTP status 401
  let Unauthorized value =
    Helper.make HttpStatusCode.Unauthorized value

  /// HTTP status 402
  let PaymentRequired value =
    Helper.make HttpStatusCode.PaymentRequired value

  /// HTTP status 403
  let Forbidden value =
    Helper.make HttpStatusCode.Forbidden value

  /// HTTP status 404
  let NotFound value = 
    Helper.make HttpStatusCode.NotFound value

  /// HTTP status 405
  let MethodNotAllowed value =
    Helper.make HttpStatusCode.MethodNotAllowed value

  /// HTTP status 406
  let NotAcceptable value =
    Helper.make HttpStatusCode.NotAcceptable value

  /// HTTP status 407
  let ProxyAuthenticationRequired value = 
    Helper.make HttpStatusCode.ProxyAuthenticationRequired value

  /// HTTP status 408
  let RequestTimeout value = 
    Helper.make HttpStatusCode.RequestTimeout value

  /// HTTP status 409
  let Conflict value =
    Helper.make HttpStatusCode.Conflict value

  /// HTTP status 410
  let Gone value =
    Helper.make HttpStatusCode.Gone value

  /// HTTP status 411
  let LengthRequired value =
    Helper.make HttpStatusCode.LengthRequired value

  /// HTTP status 412
  let PreconditionFailed value =
    Helper.make HttpStatusCode.PreconditionFailed value

  /// HTTP status 413
  let RequestEntityTooLarge value =
    Helper.make HttpStatusCode.RequestEntityTooLarge value

  /// HTTP status 414
  let RequestUriTooLong value =
    Helper.make HttpStatusCode.RequestUriTooLong value

  /// HTTP status 415
  let UnsupportedMediaType value = 
    Helper.make HttpStatusCode.UnsupportedMediaType value

  /// HTTP status 416
  let RequestedRangeNotSatisfiable value = 
    Helper.make HttpStatusCode.RequestedRangeNotSatisfiable value

  /// HTTP status 417
  let ExpectationFailed value =
    Helper.make HttpStatusCode.ExpectationFailed value

  /// HTTP status 426
  let UpgradeRequired value =
    Helper.make (unbox<HttpStatusCode> (box 426)) value

  /// HTTP status 500
  let InternalServerError value =
    Helper.make HttpStatusCode.InternalServerError value

  /// HTTP status 501
  let NotImplemented value =
    Helper.make HttpStatusCode.NotImplemented value

  /// HTTP status 502
  let BadGateway value =
    Helper.make HttpStatusCode.BadGateway value

  /// HTTP status 503
  let ServiceUnavailable value = 
    Helper.make HttpStatusCode.ServiceUnavailable value

  /// HTTP status 504
  let GatewayTimeout value = 
    Helper.make HttpStatusCode.GatewayTimeout value

  /// HTTP status 505
  let HttpVersionNotSupported value = 
    Helper.make HttpStatusCode.HttpVersionNotSupported value
