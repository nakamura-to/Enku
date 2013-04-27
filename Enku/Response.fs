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

  let headers manipulators (Response builder) = Response(fun (req) ->
    let res = builder req
    List.iter (fun manipulator -> manipulator res) manipulators 
    res)

  let setReasonPhrase reasonPhrase (Response builder) = Response(fun (req) ->
    let res = builder req
    res.ReasonPhrase <- reasonPhrase
    res)

  let setVersion version (Response builder) = Response(fun (req) ->
    let res = builder req
    res.Version <- version
    res)

  let make statusCode value = Response(fun (req) ->
    match box value with
    | :? exn as exn-> 
      req.CreateErrorResponse(statusCode, exn)
    | _ ->
      req.CreateResponse(statusCode, value) )

  /// HTTP status 100
  let Continue value = 
    make HttpStatusCode.Continue value
  /// HTTP status 100
  let``100`` value = 
    make HttpStatusCode.Continue value

  /// HTTP status 101
  let SwitchingProtocols value = 
    make HttpStatusCode.SwitchingProtocols value
  /// HTTP status 101
  let ``101`` value =
    make HttpStatusCode.SwitchingProtocols value

  /// HTTP status 200
  let Ok value =
    make HttpStatusCode.OK value
  /// HTTP status 200
  let ``200`` value =
    make HttpStatusCode.OK value

  /// HTTP status 201
  let Created value =
    make HttpStatusCode.Created value
  /// HTTP status 201
  let ``201`` value =
    make HttpStatusCode.Created value

  /// HTTP status 202
  let Accepted value =
    make HttpStatusCode.Accepted value
  /// HTTP status 202
  let ``202`` value =
    make HttpStatusCode.Accepted value

  /// HTTP status 203
  let NonAuthoritativeInformation value =
    make HttpStatusCode.NonAuthoritativeInformation value
  /// HTTP status 203
  let ``203`` value =
    make HttpStatusCode.NonAuthoritativeInformation value

  /// HTTP status 204
  let NoContent value =
    make HttpStatusCode.NoContent value
  /// HTTP status 204
  let ``204`` value =
    make HttpStatusCode.NoContent value

  /// HTTP status 205
  let ResetContent value =
    make HttpStatusCode.ResetContent value
  /// HTTP status 205
  let ``205`` value =
    make HttpStatusCode.ResetContent value

  /// HTTP status 206
  let PartialContent value =
    make HttpStatusCode.PartialContent value
  /// HTTP status 206
  let ``206`` value =
    make HttpStatusCode.PartialContent value

  /// HTTP status 300
  let Ambiguous value =
    make HttpStatusCode.Ambiguous value
  /// HTTP status 300
  let MultipleChoices value =
    make HttpStatusCode.MultipleChoices value
  /// HTTP status 300
  let ``300`` value =
    make HttpStatusCode.MultipleChoices value

  /// HTTP status 301
  let Moved value =
    make HttpStatusCode.Moved value
  /// HTTP status 301
  let MovedPermanently value =
    make HttpStatusCode.MovedPermanently value
  /// HTTP status 301
  let ``301`` value =
    make HttpStatusCode.MovedPermanently value

  /// HTTP status 302
  let Found value =
    make HttpStatusCode.Found value
  /// HTTP status 302
  let Redirect value =
    make HttpStatusCode.Redirect value
  /// HTTP status 302
  let ``302`` value =
    make HttpStatusCode.Redirect value

  /// HTTP status 303
  let RedirectMethod value = 
    make HttpStatusCode.RedirectMethod value
  /// HTTP status 303
  let SeeOther value =
    make HttpStatusCode.SeeOther value
  /// HTTP status 303
  let ``303`` value =
    make HttpStatusCode.SeeOther value

  /// HTTP status 304
  let NotModified value =
    make HttpStatusCode.NotModified value
  let ``304`` value =
    make HttpStatusCode.NotModified value

  /// HTTP status 305
  let UseProxy value =
    make HttpStatusCode.UseProxy value
  let ``305`` value =
    make HttpStatusCode.UseProxy value

  /// HTTP status 306
  let Unused value =
    make HttpStatusCode.Unused value
  let ``306`` value =
    make HttpStatusCode.Unused value

  /// HTTP status 307
  let RedirectKeepVerb value =
    make HttpStatusCode.RedirectKeepVerb value
  /// HTTP status 307
  let TemporaryRedirect value =
    make HttpStatusCode.TemporaryRedirect value
  /// HTTP status 307
  let ``307`` value =
    make HttpStatusCode.TemporaryRedirect value

  /// HTTP status 400
  let BadRequest value =
    make HttpStatusCode.BadRequest value
  /// HTTP status 400
  let ``400`` value =
    make HttpStatusCode.BadRequest value

  /// HTTP status 401
  let Unauthorized value =
    make HttpStatusCode.Unauthorized value
  /// HTTP status 401
  let ``401`` value =
    make HttpStatusCode.Unauthorized value

  /// HTTP status 402
  let PaymentRequired value =
    make HttpStatusCode.PaymentRequired value
  let ``402`` value =
    make HttpStatusCode.PaymentRequired value

  /// HTTP status 403
  let Forbidden value =
    make HttpStatusCode.Forbidden value
  /// HTTP status 403
  let ``403`` value =
    make HttpStatusCode.Forbidden value

  /// HTTP status 404
  let NotFound value = 
    make HttpStatusCode.NotFound value
  /// HTTP status 404
  let ``404`` value =
    make HttpStatusCode.NotFound value

  /// HTTP status 405
  let MethodNotAllowed value =
    make HttpStatusCode.MethodNotAllowed value
  /// HTTP status 405
  let ``405`` value =
    make HttpStatusCode.MethodNotAllowed value

  /// HTTP status 406
  let NotAcceptable value =
    make HttpStatusCode.NotAcceptable value
  /// HTTP status 406
  let ``406`` value =
    make HttpStatusCode.NotAcceptable value

  /// HTTP status 407
  let ProxyAuthenticationRequired value = 
    make HttpStatusCode.ProxyAuthenticationRequired value
  /// HTTP status 407
  let ``407`` value = 
    make HttpStatusCode.ProxyAuthenticationRequired value

  /// HTTP status 408
  let RequestTimeout value = 
    make HttpStatusCode.RequestTimeout value
  /// HTTP status 408
  let ``408`` value = 
    make HttpStatusCode.RequestTimeout value

  /// HTTP status 409
  let Conflict value =
    make HttpStatusCode.Conflict value
  /// HTTP status 409
  let ``409`` value =
    make HttpStatusCode.Conflict value

  /// HTTP status 410
  let Gone value =
    make HttpStatusCode.Gone value
  /// HTTP status 410
  let ``410`` value =
    make HttpStatusCode.Gone value

  /// HTTP status 411
  let LengthRequired value =
    make HttpStatusCode.LengthRequired value
  /// HTTP status 411
  let ``411`` value =
    make HttpStatusCode.LengthRequired value

  /// HTTP status 412
  let PreconditionFailed value =
    make HttpStatusCode.PreconditionFailed value
  /// HTTP status 412
  let ``412`` value =
    make HttpStatusCode.PreconditionFailed value

  /// HTTP status 413
  let RequestEntityTooLarge value =
    make HttpStatusCode.RequestEntityTooLarge value
  /// HTTP status 413
  let ``413`` value =
    make HttpStatusCode.RequestEntityTooLarge value

  /// HTTP status 414
  let RequestUriTooLong value =
    make HttpStatusCode.RequestUriTooLong value
  /// HTTP status 414
  let ``414`` value =
    make HttpStatusCode.RequestUriTooLong value

  /// HTTP status 415
  let UnsupportedMediaType value = 
    make HttpStatusCode.UnsupportedMediaType value
  /// HTTP status 415
  let ``415`` value = 
    make HttpStatusCode.UnsupportedMediaType value

  /// HTTP status 416
  let RequestedRangeNotSatisfiable value = 
    make HttpStatusCode.RequestedRangeNotSatisfiable value
  /// HTTP status 416
  let ``416`` value = 
    make HttpStatusCode.RequestedRangeNotSatisfiable value

  /// HTTP status 417
  let ExpectationFailed value =
    make HttpStatusCode.ExpectationFailed value
  /// HTTP status 417
  let ``417`` value =
    make HttpStatusCode.ExpectationFailed value

  /// HTTP status 426
  let UpgradeRequired value =
    make (unbox<HttpStatusCode> (box 426)) value
  /// HTTP status 426
  let ``426`` value =
    make (unbox<HttpStatusCode> (box 426)) value

  /// HTTP status 500
  let InternalServerError value =
    make HttpStatusCode.InternalServerError value
  /// HTTP status 500
  let ``500`` value =
    make HttpStatusCode.InternalServerError value

  /// HTTP status 501
  let NotImplemented value =
    make HttpStatusCode.NotImplemented value
  /// HTTP status 501
  let ``501`` value =
    make HttpStatusCode.NotImplemented value

  /// HTTP status 502
  let BadGateway value =
    make HttpStatusCode.BadGateway value
  let ``502`` value =
  /// HTTP status 502
    make HttpStatusCode.BadGateway value

  /// HTTP status 503
  let ServiceUnavailable value = 
    make HttpStatusCode.ServiceUnavailable value
  /// HTTP status 503
  let ``503`` value = 
    make HttpStatusCode.ServiceUnavailable value

  /// HTTP status 504
  let GatewayTimeout value = 
    make HttpStatusCode.GatewayTimeout value
  /// HTTP status 504
  let ``504`` value = 
    make HttpStatusCode.GatewayTimeout value

  /// HTTP status 505
  let HttpVersionNotSupported value = 
    make HttpStatusCode.HttpVersionNotSupported value
  /// HTTP status 505
  let ``505`` value = 
    make HttpStatusCode.HttpVersionNotSupported value