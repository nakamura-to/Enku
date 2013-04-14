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

  exception internal Exit of Response

  let exit response =
    raise <| Exit response

  let appendHeaders headers (Response builder) = Response(fun (requestMessage) ->
      let responseMessage = builder requestMessage
      headers |> List.iter (fun header -> header responseMessage)
      responseMessage)

  let private makeResponse statusCode value = Response(fun (requestMessage) ->
    match box value with
    | :? exn as exn-> 
      requestMessage.CreateErrorResponse(statusCode, exn)
    | _ ->
      requestMessage.CreateResponse(statusCode, value) )

  /// HTTP status 100
  let Continue value = 
    makeResponse HttpStatusCode.Continue value
  /// HTTP status 100
  let``100`` value = 
    makeResponse HttpStatusCode.Continue value

  /// HTTP status 101
  let SwitchingProtocols value = 
    makeResponse HttpStatusCode.SwitchingProtocols value
  /// HTTP status 101
  let ``101`` value =
    makeResponse HttpStatusCode.SwitchingProtocols value

  /// HTTP status 200
  let OK value =
    makeResponse HttpStatusCode.OK value
  /// HTTP status 200
  let ``200`` value =
    makeResponse HttpStatusCode.OK value

  /// HTTP status 201
  let Created value =
    makeResponse HttpStatusCode.Created value
  /// HTTP status 201
  let ``201`` value =
    makeResponse HttpStatusCode.Created value

  /// HTTP status 202
  let Accepted value =
    makeResponse HttpStatusCode.Accepted value
  /// HTTP status 202
  let ``202`` value =
    makeResponse HttpStatusCode.Accepted value

  /// HTTP status 203
  let NonAuthoritativeInformation value =
    makeResponse HttpStatusCode.NonAuthoritativeInformation value
  /// HTTP status 203
  let ``203`` value =
    makeResponse HttpStatusCode.NonAuthoritativeInformation value

  /// HTTP status 204
  let NoContent value =
    makeResponse HttpStatusCode.NoContent value
  /// HTTP status 204
  let ``204`` value =
    makeResponse HttpStatusCode.NoContent value

  /// HTTP status 205
  let ResetContent value =
    makeResponse HttpStatusCode.ResetContent value
  /// HTTP status 205
  let ``205`` value =
    makeResponse HttpStatusCode.ResetContent value

  /// HTTP status 206
  let PartialContent value =
    makeResponse HttpStatusCode.PartialContent value
  /// HTTP status 206
  let ``206`` value =
    makeResponse HttpStatusCode.PartialContent value

  /// HTTP status 300
  let Ambiguous value =
    makeResponse HttpStatusCode.Ambiguous value
  /// HTTP status 300
  let MultipleChoices value =
    makeResponse HttpStatusCode.MultipleChoices value
  /// HTTP status 300
  let ``300`` value =
    makeResponse HttpStatusCode.MultipleChoices value

  /// HTTP status 301
  let Moved value =
    makeResponse HttpStatusCode.Moved value
  /// HTTP status 301
  let MovedPermanently value =
    makeResponse HttpStatusCode.MovedPermanently value
  /// HTTP status 301
  let ``301`` value =
    makeResponse HttpStatusCode.MovedPermanently value

  /// HTTP status 302
  let Found value =
    makeResponse HttpStatusCode.Found value
  /// HTTP status 302
  let Redirect value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.Redirect value
  /// HTTP status 302
  let ``302`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.Redirect value

  /// HTTP status 303
  let RedirectMethod value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RedirectMethod value
  /// HTTP status 303
  let SeeOther value =
    makeResponse HttpStatusCode.SeeOther value
  /// HTTP status 303
  let ``303`` value =
    makeResponse HttpStatusCode.SeeOther value

  /// HTTP status 304
  let NotModified value =
    makeResponse HttpStatusCode.NotModified value
  let ``304`` value =
    makeResponse HttpStatusCode.NotModified value

  /// HTTP status 305
  let UseProxy value =
    makeResponse HttpStatusCode.UseProxy value
  let ``305`` value =
    makeResponse HttpStatusCode.UseProxy value

  /// HTTP status 306
  let Unused value =
    makeResponse HttpStatusCode.Unused value
  let ``306`` value =
    makeResponse HttpStatusCode.Unused value

  /// HTTP status 307
  let RedirectKeepVerb value =
    makeResponse HttpStatusCode.RedirectKeepVerb value
  /// HTTP status 307
  let TemporaryRedirect value =
    makeResponse HttpStatusCode.TemporaryRedirect value
  /// HTTP status 307
  let ``307`` value =
    makeResponse HttpStatusCode.TemporaryRedirect value

  /// HTTP status 400
  let BadRequest value =
    makeResponse HttpStatusCode.BadRequest value
  /// HTTP status 400
  let ``400`` value =
    makeResponse HttpStatusCode.BadRequest value

  /// HTTP status 401
  let Unauthorized value =
    makeResponse HttpStatusCode.Unauthorized value
  /// HTTP status 401
  let ``401`` value =
    makeResponse HttpStatusCode.Unauthorized value

  /// HTTP status 402
  let PaymentRequired value =
    makeResponse HttpStatusCode.PaymentRequired value
  let ``402`` value =
    makeResponse HttpStatusCode.PaymentRequired value

  /// HTTP status 403
  let Forbidden value =
    makeResponse HttpStatusCode.Forbidden value
  /// HTTP status 403
  let ``403`` value =
    makeResponse HttpStatusCode.Forbidden value

  /// HTTP status 404
  let NotFound value = 
    makeResponse HttpStatusCode.NotFound value
  /// HTTP status 404
  let ``404`` value =
    makeResponse HttpStatusCode.NotFound value

  /// HTTP status 405
  let MethodNotAllowed value =
    makeResponse HttpStatusCode.MethodNotAllowed value
  /// HTTP status 405
  let ``405`` value =
    makeResponse HttpStatusCode.MethodNotAllowed value

  /// HTTP status 406
  let NotAcceptable value =
    makeResponse HttpStatusCode.NotAcceptable value
  /// HTTP status 406
  let ``406`` value =
    makeResponse HttpStatusCode.NotAcceptable value

  /// HTTP status 407
  let ProxyAuthenticationRequired value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.ProxyAuthenticationRequired value
  /// HTTP status 407
  let ``407`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.ProxyAuthenticationRequired value

  /// HTTP status 408
  let RequestTimeout value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestTimeout value
  /// HTTP status 408
  let ``408`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestTimeout value

  /// HTTP status 409
  let Conflict value =
    makeResponse HttpStatusCode.Conflict value
  /// HTTP status 409
  let ``409`` value =
    makeResponse HttpStatusCode.Conflict value

  /// HTTP status 410
  let Gone value =
    makeResponse HttpStatusCode.Gone value
  /// HTTP status 410
  let ``410`` value =
    makeResponse HttpStatusCode.Gone value

  /// HTTP status 411
  let LengthRequired value =
    makeResponse HttpStatusCode.LengthRequired value
  /// HTTP status 411
  let ``411`` value =
    makeResponse HttpStatusCode.LengthRequired value

  /// HTTP status 412
  let PreconditionFailed value =
    makeResponse HttpStatusCode.PreconditionFailed value
  /// HTTP status 412
  let ``412`` value =
    makeResponse HttpStatusCode.PreconditionFailed value

  /// HTTP status 413
  let RequestEntityTooLarge value =
    makeResponse HttpStatusCode.RequestEntityTooLarge value
  /// HTTP status 413
  let ``413`` value =
    makeResponse HttpStatusCode.RequestEntityTooLarge value

  /// HTTP status 414
  let RequestUriTooLong value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestUriTooLong value
  /// HTTP status 414
  let ``414`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestUriTooLong value

  /// HTTP status 415
  let UnsupportedMediaType value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.UnsupportedMediaType value
  /// HTTP status 415
  let ``415`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.UnsupportedMediaType value

  /// HTTP status 416
  let RequestedRangeNotSatisfiable value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestedRangeNotSatisfiable value
  /// HTTP status 416
  let ``416`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.RequestedRangeNotSatisfiable value

  /// HTTP status 417
  let ExpectationFailed value =
    makeResponse HttpStatusCode.ExpectationFailed value
  /// HTTP status 417
  let ``417`` value =
    makeResponse HttpStatusCode.ExpectationFailed value

  /// HTTP status 426
  let UpgradeRequired value =
    makeResponse (unbox<HttpStatusCode> (box 426)) value
  /// HTTP status 426
  let ``426`` value =
    makeResponse (unbox<HttpStatusCode> (box 426)) value

  /// HTTP status 500
  let InternalServerError value =
    makeResponse HttpStatusCode.InternalServerError value
  /// HTTP status 500
  let ``500`` value =
    makeResponse HttpStatusCode.InternalServerError value

  /// HTTP status 501
  let NotImplemented value =
    makeResponse HttpStatusCode.NotImplemented value
  /// HTTP status 501
  let ``501`` value =
    makeResponse HttpStatusCode.NotImplemented value

  /// HTTP status 502
  let BadGateway value =
    makeResponse HttpStatusCode.BadGateway value
  let ``502`` value =
  /// HTTP status 502
    makeResponse HttpStatusCode.BadGateway value

  /// HTTP status 503
  let ServiceUnavailable value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.ServiceUnavailable value
  /// HTTP status 503
  let ``503`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.ServiceUnavailable value

  /// HTTP status 504
  let GatewayTimeout value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.GatewayTimeout value
  /// HTTP status 504
  let ``504`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.GatewayTimeout value

  /// HTTP status 505
  let HttpVersionNotSupported value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.HttpVersionNotSupported value
  /// HTTP status 505
  let ``505`` value = fun (reqMessage: HttpRequestMessage) ->
    makeResponse HttpStatusCode.HttpVersionNotSupported value