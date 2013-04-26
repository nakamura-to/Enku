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
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text.RegularExpressions
open System.Web.Http

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Routing =

  exception Exit of Response

  let private regex = Regex(@"{\?(?<optional>[^}]*)}")

  let parsePath path =
    let defaults = Dictionary<string, obj>()
    let path = regex.Replace(path, fun (m: Match) ->
      let key = m.Groups.["optional"].Value
      if defaults.ContainsKey(key) then
        defaults.Remove(key) |> ignore
      defaults.Add(key, RouteParameter.Optional)
      "{" + key + "}")
    path, defaults

  let makeHandler controller errorHandler = 
    { new HttpMessageHandler() with
      override this.SendAsync(reqMessage, token) = 
        let req = Request reqMessage
        let actions = controller req
        let computation = async {
          try
            let! (Response builder) =
              actions
              |> List.tryPick (fun (action, body) -> 
                Action.run req body action)
              |> function 
              | Some result -> result
              | _ -> async { return Response.NotFound "" }
            return builder reqMessage
          with e ->
            let (Response builder) = 
              match e with 
              | Exit builder ->
                builder
              | _ ->
                errorHandler req e
            return builder reqMessage }
        Async.StartAsTask(computation, cancellationToken = token) }

  let route (config: HttpConfiguration) (basePath: string) (router: Router) =
    let (controllers: (string * Controller) list), (errorHandler: ErrorHandler) = router()
    controllers
    |> List.iter (fun (path, controller) ->
      let path =
        if basePath.EndsWith("/") then
          basePath + path
        else
          basePath + "/" + path
      let path, defaults = parsePath path
      let handler = makeHandler controller errorHandler
      config.Routes.MapHttpRoute(
        name = path,
        routeTemplate = path,
        defaults = defaults,
        constraints = null,
        handler = handler) |> ignore)

  let exit response =
    raise <| Exit response

  let addGlobalHandler (config: HttpConfiguration) handler =
    config.MessageHandlers.Add <|
      { new DelegatingHandler() with
        override this.SendAsync(request, token) =
          let response = Async.AwaitTask <| base.SendAsync(request, token)
          let computation = handler request response
          Async.StartAsTask(computation, cancellationToken = token) }
