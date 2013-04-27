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
open System.Net.Http
open System.Net.Http.Formatting

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Request =

  module Helper =

    type FormatError(message: string, innerException: exn) =
      inherit Exception(message, innerException)

    type FormatterLogger(errors: ResizeArray<exn>) =
      interface IFormatterLogger with
        member this.LogError(errorPath: string, errorMessage: string) = 
          if errorPath = null then invalidArg "errorPath" "The arg must not null"
          if errorMessage = null then invalidArg "errorMessage" "The arg must not be null"
          errors.Add(FormatError(errorPath, Exception(errorMessage)))
        member this.LogError(errorPath: string, exn: exn) =
          if errorPath = null then invalidArg "errorPath" "The arg must not null"
          if exn = null then invalidArg "exn" "The arg must not be null"
          errors.Add(FormatError(errorPath, exn))

    let toKeyValuesMap keyValuePairs =
      keyValuePairs
      |> Seq.groupBy (fun (KeyValue(key, _)) -> key)
      |> Seq.map (fun (key, values) ->
        let values =
          values 
          |> Seq.map (fun (KeyValue(_, value)) -> value) 
          |> Seq.toList
        key, values)
      |> Map.ofSeq


  let asyncReadAsString (Request req) =
    Async.AwaitTask <| req.Content.ReadAsStringAsync()

  let asyncReadAsStream (Request req) =
    Async.AwaitTask <| req.Content.ReadAsStreamAsync()

  let asyncReadAsBytes (Request req) =
    Async.AwaitTask <| req.Content.ReadAsByteArrayAsync()

  let asyncReadAs<'T> (Request req) = async {
    let formatters = req.GetConfiguration().Formatters
    let errors = ResizeArray()
    let logger = Helper.FormatterLogger(errors)
    let! result = Async.AwaitTask <| req.Content.ReadAsAsync<'T>(formatters, logger)
    let errors = Seq.toList errors
    match errors with
    | [] -> return Result.Ok result
    | head :: tail -> return Result.Error (head, tail) }

  let asyncReadAsForm req = async {
    let! result = asyncReadAs<FormDataCollection> req
    match result with
    | Result.Ok form -> return Result.Ok <| Helper.toKeyValuesMap form
    | Result.Error (head, tail) -> return Result.Error (head, tail) }

  let getQueryString key (Request req) = 
    req.GetQueryNameValuePairs()
    |> Seq.tryPick (fun (KeyValue(k, v)) -> 
      if k = key then Some v 
      else None)

  let getQueryStringMap (Request req) = 
    req.GetQueryNameValuePairs()
    |> Seq.distinctBy (fun (KeyValue(k, _)) -> k)
    |> Seq.map (fun (KeyValue(k, v)) -> k, v)
    |> Map.ofSeq

  let getQueryStringsAll (Request req) = 
    req.GetQueryNameValuePairs()
    |> Helper.toKeyValuesMap

  let getRouteValue key (Request req) =
    let routeData = req.GetRouteData()
    match routeData.Values.TryGetValue(key) with
    | true, v ->
      if v = null then None 
      else Some <| string v
    | _ -> 
      None

  let getRouteValueMap (Request req) =
    let routeData = req.GetRouteData()
    routeData.Values
    |> Seq.choose (fun (KeyValue(k, v)) -> 
      if v = null then None 
      else Some (k, string v))
    |> Map.ofSeq

  let getMethod (Request req) =
    req.Method

  let getRequestUri (Request req) =
    req.RequestUri

  let getVersion (Request req) =
    req.Version