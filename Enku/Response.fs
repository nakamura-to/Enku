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
    
    let make statusCode value mediaType = Response(fun req ->
      match box value with
      | :? exn as exn-> 
        req.CreateErrorResponse(statusCode, exn)
      | _ ->
        match mediaType with
        | MediaType.Neg ->
          req.CreateResponse(statusCode, value)
        | MediaType.Name mediaType ->
          req.CreateResponse(statusCode, value, mediaType)
        | MediaType.Formatter formatter ->
          req.CreateResponse(statusCode, value, formatter) )

  let headers manipulators (Response builder) = Response(fun req ->
    let res = builder req
    List.iter (fun manipulator -> manipulator res) manipulators 
    res)

  let setReasonPhrase reasonPhrase (Response builder) = Response(fun req ->
    let res = builder req
    res.ReasonPhrase <- reasonPhrase
    res)

  let setVersion version (Response builder) = Response(fun req ->
    let res = builder req
    res.Version <- version
    res)

  let make (statusCode: int) value mediaType =
    Helper.make (unbox<HttpStatusCode> (box statusCode)) value mediaType

  /// HTTP status 100
  let Continue value mediaType = 
    Helper.make HttpStatusCode.Continue value mediaType

  /// HTTP status 101
  let SwitchingProtocols value mediaType = 
    Helper.make HttpStatusCode.SwitchingProtocols value mediaType

  /// HTTP status 200
  let Ok value mediaType =
    Helper.make HttpStatusCode.OK value mediaType

  /// HTTP status 201
  let Created value mediaType =
    Helper.make HttpStatusCode.Created value mediaType

  /// HTTP status 202
  let Accepted value mediaType =
    Helper.make HttpStatusCode.Accepted value mediaType

  /// HTTP status 203
  let NonAuthoritativeInformation value mediaType =
    Helper.make HttpStatusCode.NonAuthoritativeInformation value mediaType

  /// HTTP status 204
  let NoContent value mediaType =
    Helper.make HttpStatusCode.NoContent value mediaType

  /// HTTP status 205
  let ResetContent value mediaType =
    Helper.make HttpStatusCode.ResetContent value mediaType

  /// HTTP status 206
  let PartialContent value mediaType =
    Helper.make HttpStatusCode.PartialContent value mediaType

  /// HTTP status 300
  let Ambiguous value mediaType =
    Helper.make HttpStatusCode.Ambiguous value mediaType

  /// HTTP status 300
  let MultipleChoices value mediaType =
    Helper.make HttpStatusCode.MultipleChoices value mediaType

  /// HTTP status 301
  let Moved value mediaType =
    Helper.make HttpStatusCode.Moved value

  /// HTTP status 301
  let MovedPermanently value mediaType =
    Helper.make HttpStatusCode.MovedPermanently value mediaType

  /// HTTP status 302
  let Found value mediaType =
    Helper.make HttpStatusCode.Found value mediaType

  /// HTTP status 302
  let Redirect value mediaType =
    Helper.make HttpStatusCode.Redirect value mediaType

  /// HTTP status 303
  let RedirectMethod value mediaType = 
    Helper.make HttpStatusCode.RedirectMethod value mediaType

  /// HTTP status 303
  let SeeOther value mediaType =
    Helper.make HttpStatusCode.SeeOther value mediaType

  /// HTTP status 304
  let NotModified value mediaType =
    Helper.make HttpStatusCode.NotModified value mediaType

  /// HTTP status 305
  let UseProxy value mediaType =
    Helper.make HttpStatusCode.UseProxy value mediaType

  /// HTTP status 306
  let Unused value mediaType =
    Helper.make HttpStatusCode.Unused value mediaType

  /// HTTP status 307
  let RedirectKeepVerb value mediaType =
    Helper.make HttpStatusCode.RedirectKeepVerb value mediaType

  /// HTTP status 307
  let TemporaryRedirect value mediaType =
    Helper.make HttpStatusCode.TemporaryRedirect value mediaType

  /// HTTP status 400
  let BadRequest value mediaType =
    Helper.make HttpStatusCode.BadRequest value mediaType

  /// HTTP status 401
  let Unauthorized value mediaType =
    Helper.make HttpStatusCode.Unauthorized value mediaType

  /// HTTP status 402
  let PaymentRequired value mediaType =
    Helper.make HttpStatusCode.PaymentRequired value mediaType

  /// HTTP status 403
  let Forbidden value mediaType =
    Helper.make HttpStatusCode.Forbidden value mediaType

  /// HTTP status 404
  let NotFound value mediaType = 
    Helper.make HttpStatusCode.NotFound value mediaType

  /// HTTP status 405
  let MethodNotAllowed value mediaType =
    Helper.make HttpStatusCode.MethodNotAllowed value mediaType

  /// HTTP status 406
  let NotAcceptable value mediaType =
    Helper.make HttpStatusCode.NotAcceptable value mediaType

  /// HTTP status 407
  let ProxyAuthenticationRequired value mediaType = 
    Helper.make HttpStatusCode.ProxyAuthenticationRequired value mediaType

  /// HTTP status 408
  let RequestTimeout value mediaType = 
    Helper.make HttpStatusCode.RequestTimeout value mediaType

  /// HTTP status 409
  let Conflict value mediaType =
    Helper.make HttpStatusCode.Conflict value mediaType

  /// HTTP status 410
  let Gone value mediaType =
    Helper.make HttpStatusCode.Gone value mediaType

  /// HTTP status 411
  let LengthRequired value mediaType =
    Helper.make HttpStatusCode.LengthRequired value mediaType

  /// HTTP status 412
  let PreconditionFailed value mediaType =
    Helper.make HttpStatusCode.PreconditionFailed value mediaType

  /// HTTP status 413
  let RequestEntityTooLarge value mediaType =
    Helper.make HttpStatusCode.RequestEntityTooLarge value mediaType

  /// HTTP status 414
  let RequestUriTooLong value mediaType =
    Helper.make HttpStatusCode.RequestUriTooLong value mediaType

  /// HTTP status 415
  let UnsupportedMediaType value mediaType = 
    Helper.make HttpStatusCode.UnsupportedMediaType value mediaType

  /// HTTP status 416
  let RequestedRangeNotSatisfiable value mediaType = 
    Helper.make HttpStatusCode.RequestedRangeNotSatisfiable value mediaType

  /// HTTP status 417
  let ExpectationFailed value mediaType =
    Helper.make HttpStatusCode.ExpectationFailed value mediaType

  /// HTTP status 426
  let UpgradeRequired value mediaType =
    Helper.make (unbox<HttpStatusCode> (box 426)) value mediaType

  /// HTTP status 500
  let InternalServerError value mediaType =
    Helper.make HttpStatusCode.InternalServerError value mediaType

  /// HTTP status 501
  let NotImplemented value mediaType =
    Helper.make HttpStatusCode.NotImplemented value mediaType

  /// HTTP status 502
  let BadGateway value mediaType =
    Helper.make HttpStatusCode.BadGateway value mediaType

  /// HTTP status 503
  let ServiceUnavailable value mediaType = 
    Helper.make HttpStatusCode.ServiceUnavailable value mediaType

  /// HTTP status 504
  let GatewayTimeout value mediaType = 
    Helper.make HttpStatusCode.GatewayTimeout value mediaType

  /// HTTP status 505
  let HttpVersionNotSupported value mediaType = 
    Helper.make HttpStatusCode.HttpVersionNotSupported value mediaType
