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
open System.Collections
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Web.Http
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher
open Microsoft.FSharp.Reflection

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Routing =

  let internal makeHandler controller = 
    { new HttpMessageHandler() with
      override this.SendAsync(request, token) = 
        let req = Request request
        let res = Response request
        let operation = async {
          let! response = 
            controller ()
            |> List.tryPick (fun (action, operation) ->
              match Action.run req res operation action with
              | Completion operation -> Some operation
              | Skip -> None)
            |> function 
            | Some operation -> operation
            | _ -> async {return new HttpResponseMessage(HttpStatusCode.NotFound)}
          return response }
        Async.StartAsTask(computation = operation, cancellationToken = token) }
  
  let regex = Regex(@"{\?(?<optional>[^}]*)}") 

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