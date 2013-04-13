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

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Routing =

  let internal makeHandler controller = 
    { new HttpMessageHandler() with
      override this.SendAsync(reqMessage, token) = 
        let req = Request reqMessage
        let runAction (action, body) = 
          match Action.run req body action with
          | Right r -> Some r
          | Left _ -> None
        let actions, (errHandler: ErrorHandler) = controller ()
        let computation = async {
          try
            let! resMessageBuilder =
              actions
              |> List.tryPick runAction
              |> function 
              | Some result -> result
              | _ -> async { return fun _ -> new HttpResponseMessage(HttpStatusCode.NotFound) }
            return resMessageBuilder reqMessage
          with e ->
            let! resMessageBuilder = 
              match e with 
              | Response.Exit resMessageBuilder ->
                resMessageBuilder
              | _ ->
                errHandler req e
            return resMessageBuilder reqMessage }
        Async.StartAsTask(computation = computation, cancellationToken = token) }
  
  let internal regex = Regex(@"{\?(?<optional>[^}]*)}") 

  let route (config: HttpConfiguration) url controller =
    let defaults = Dictionary<string, obj>()
    let url = regex.Replace(url, fun (m: Match) ->
      let key = m.Groups.["optional"].Value
      if defaults.ContainsKey(key) then
        defaults.Remove(key) |> ignore
      defaults.Add(key, RouteParameter.Optional)
      "{" + key + "}")
    config.Routes.MapHttpRoute(
      name = url,
      routeTemplate = url,
      defaults = defaults,
      constraints = null,
      handler = makeHandler controller) |> ignore