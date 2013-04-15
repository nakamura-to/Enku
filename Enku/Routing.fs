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
      override this.SendAsync(reqMessage, cancellationToken) = 
        let req = Request reqMessage
        let actions, (errHandler: ErrorHandler) = controller req
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
              | Response.Exit builder ->
                builder
              | _ ->
                errHandler req e
            return builder reqMessage }
        Async.StartAsTask(computation, cancellationToken = cancellationToken) }
  
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