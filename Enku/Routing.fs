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

  module Helper =

    let regex = Regex(@"{\?(?<optional>[^}]*)}")

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
      let pickAction req =
        let actionDefs = controller req
        actionDefs
        |> List.tryPick (fun (constraint_, action) -> 
          if constraint_ req then Some action
          else None) 
        |> function 
        | Some action -> action
        | _ -> fun req -> async { return Response.NotFound "" }
    
      { new HttpMessageHandler() with
        override this.SendAsync(request, token) = 
          let req = Request request
          let action = pickAction req
          let computation = async {
            try
              let! (Response builder) = action req
              return builder request
            with e ->
              let (Response builder) = errorHandler req e
              return builder request }
          Async.StartAsTask(computation, cancellationToken = token) }


  let addGlobalHandler (config: HttpConfiguration) handler =
    config.MessageHandlers.Add <|
      { new DelegatingHandler() with
        override this.SendAsync(request, token) =
          let response = Async.AwaitTask <| base.SendAsync(request, token)
          let computation = handler request response
          Async.StartAsTask(computation, cancellationToken = token) }

  let route (config: HttpConfiguration) (basePath: string) (router: Router) =
    let (controllerDefs: ControllerDef list), (errorHandler: ErrorHandler) = router()
    controllerDefs
    |> List.iter (fun (path, controller) ->
      let path, defaults = Helper.parsePath (basePath + path)
      let handler = Helper.makeHandler controller errorHandler
      config.Routes.MapHttpRoute(
        name = path,
        routeTemplate = path,
        defaults = defaults,
        constraints = null,
        handler = handler) |> ignore)
