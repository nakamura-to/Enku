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

[<Sealed>]
type Response =

  static member private MakeResponseBuilder(statusCode, value) = fun (reqMessage: HttpRequestMessage) ->
    match box value with
    | :? exn as exn-> 
      reqMessage.CreateErrorResponse(statusCode, exn)
    | _ ->
      reqMessage.CreateResponse(statusCode, value)

  static member Continue(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Continue, value)

  static member SwitchingProtocols(value) =
    Response.MakeResponseBuilder(HttpStatusCode.SwitchingProtocols, value)

  static member OK(value) =
    Response.MakeResponseBuilder(HttpStatusCode.OK, value)

  static member Created(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Created, value)

  static member Accepted(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Accepted, value)

  static member NonAuthoritativeInformation(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NonAuthoritativeInformation, value)

  static member NoContent(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NoContent, value)

  static member ResetContent(value) =
    Response.MakeResponseBuilder(HttpStatusCode.ResetContent, value)

  static member PartialContent(value) =
    Response.MakeResponseBuilder(HttpStatusCode.PartialContent, value)

  static member Ambiguous(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Ambiguous, value)

  static member MultipleChoices(value) =
    Response.MakeResponseBuilder(HttpStatusCode.MultipleChoices, value)

  static member Moved(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Moved, value)

  static member MovedPermanently(value) =
    Response.MakeResponseBuilder(HttpStatusCode.MovedPermanently, value)

  static member Found(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Found, value)

  static member Redirect(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.Redirect, value)

  static member RedirectMethod(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.RedirectMethod, value)

  static member SeeOther(value) =
    Response.MakeResponseBuilder(HttpStatusCode.SeeOther, value)

  static member NotModified(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NotModified, value)

  static member UseProxy(value) =
    Response.MakeResponseBuilder(HttpStatusCode.UseProxy, value)

  static member Unused(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Unused, value)

  static member RedirectKeepVerb(value) =
    Response.MakeResponseBuilder(HttpStatusCode.RedirectKeepVerb, value)

  static member TemporaryRedirect(value) =
    Response.MakeResponseBuilder(HttpStatusCode.TemporaryRedirect, value)

  static member BadRequest(value) =
    Response.MakeResponseBuilder(HttpStatusCode.BadRequest, value)

  static member Unauthorized(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Unauthorized, value)

  static member PaymentRequired(value) =
    Response.MakeResponseBuilder(HttpStatusCode.PaymentRequired, value)

  static member Forbidden(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Forbidden, value)

  static member NotFound(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NotFound, value)

  static member MethodNotAllowed(value) =
    Response.MakeResponseBuilder(HttpStatusCode.MethodNotAllowed, value)

  static member NotAcceptable(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NotAcceptable, value)

  static member ProxyAuthenticationRequired(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.ProxyAuthenticationRequired, value)

  static member RequestTimeout(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.RequestTimeout, value)

  static member Conflict(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Conflict, value)

  static member Gone(value) =
    Response.MakeResponseBuilder(HttpStatusCode.Gone, value)

  static member LengthRequired(value) =
    Response.MakeResponseBuilder(HttpStatusCode.LengthRequired, value)

  static member PreconditionFailed(value) =
    Response.MakeResponseBuilder(HttpStatusCode.PreconditionFailed, value)

  static member RequestEntityTooLarge(value) =
    Response.MakeResponseBuilder(HttpStatusCode.RequestEntityTooLarge, value)

  static member RequestUriTooLong(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.RequestUriTooLong, value)

  static member UnsupportedMediaType(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.UnsupportedMediaType, value)

  static member RequestedRangeNotSatisfiable(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.RequestedRangeNotSatisfiable, value)

  static member ExpectationFailed(value) =
    Response.MakeResponseBuilder(HttpStatusCode.ExpectationFailed, value)

  static member UpgradeRequired(value) =
    Response.MakeResponseBuilder(unbox<HttpStatusCode> (box 426), value)

  static member InternalServerError(value) =
    Response.MakeResponseBuilder(HttpStatusCode.InternalServerError, value)

  static member NotImplemented(value) =
    Response.MakeResponseBuilder(HttpStatusCode.NotImplemented, value)

  static member BadGateway(value) =
    Response.MakeResponseBuilder(HttpStatusCode.BadGateway, value)

  static member ServiceUnavailable(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.ServiceUnavailable, value)

  static member GatewayTimeout(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.GatewayTimeout, value)

  static member HttpVersionNotSupported(value) = fun (reqMessage: HttpRequestMessage) ->
    Response.MakeResponseBuilder(HttpStatusCode.HttpVersionNotSupported, value)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Response =

  exception internal Exit of (Async<HttpRequestMessage -> HttpResponseMessage>)

  let exit resMessageBuilder =
    raise <| Exit (async { return resMessageBuilder })    

