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


  let asyncReadAsString (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsStringAsync()

  let asyncReadAsStream (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsStreamAsync()

  let asyncReadAsBytes (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsByteArrayAsync()

  let asyncReadAs<'T> (Request reqMessage) = async {
    let formatters = reqMessage.GetConfiguration().Formatters
    let! result = Async.AwaitTask <| reqMessage.Content.ReadAsAsync<'T>(formatters) 
    return result }

  let asyncReadAsForm req = async {
    let! form = asyncReadAs<FormDataCollection> req
    return Helper.toKeyValuesMap form }

  let asyncTryReadAs<'T> (Request reqMessage) = async {
    let formatters = reqMessage.GetConfiguration().Formatters
    let errors = ResizeArray()
    let logger = Helper.FormatterLogger(errors)
    let! result = Async.AwaitTask <| reqMessage.Content.ReadAsAsync<'T>(formatters, logger)
    let errors = Seq.toList errors
    return
      match errors with
      | [] -> Ok result
      | head :: tail -> Error (head, tail) }

  let asyncTryReadAsForm req = async {
    let! result = asyncTryReadAs<FormDataCollection> req
    return
      match result with
      | Ok form -> Ok <| Helper.toKeyValuesMap form
      | Error (head, tail) -> Error (head, tail) }

  let getQueryString key (Request reqMessage) = 
    reqMessage.GetQueryNameValuePairs()
    |> Seq.tryPick (fun (KeyValue(k, v)) -> 
      if k = key then 
        Some v 
      else 
        None)

  let getQueryStringMap (Request reqMessage) = 
    reqMessage.GetQueryNameValuePairs()
    |> Seq.distinctBy (fun (KeyValue(k, _)) -> k)
    |> Seq.map (fun (KeyValue(k, v)) -> k, v)
    |> Map.ofSeq

  let getQueryStringsAll (Request reqMessage) = 
    reqMessage.GetQueryNameValuePairs()
    |> Helper.toKeyValuesMap

  let getRouteValue key (Request reqMessage) =
    let routeData = reqMessage.GetRouteData()
    match routeData.Values.TryGetValue(key) with
    | true, v ->
      if v = null then 
        None 
      else 
        Some <| string v
    | _ -> 
      None

  let getRouteValueMap (Request reqMessage) =
    let routeData = reqMessage.GetRouteData()
    routeData.Values
    |> Seq.choose (fun (KeyValue(k, v)) -> 
      if v = null then 
        None 
      else 
        Some (k, string v))
    |> Map.ofSeq

  let getMethod (Request reqMessage) =
    reqMessage.Method

  let getRequestUri (Request reqMessage) =
    reqMessage.RequestUri

  let getVersion (Request reqMessage) =
    reqMessage.Version