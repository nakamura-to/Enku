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
      override this.SendAsync(requestMessage, token) = 
        let request = Request requestMessage
        let runAction (action, body) = 
          match Action.run request body action with
          | Right r -> Some r
          | Left _ -> None
        let actions, (errHandler: ErrorHandler) = controller request
        let computation = async {
          try
            let! (Response builder) =
              actions
              |> List.tryPick runAction
              |> function 
              | Some result -> result
              | _ -> async { return Response(fun _ -> new HttpResponseMessage(HttpStatusCode.NotFound)) }
            return builder requestMessage
          with e ->
            let (Response builder) = 
              match e with 
              | Response.Exit builder ->
                builder
              | _ ->
                errHandler request e
            return builder requestMessage }
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