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

  let private toMap keyValuePairs =
    keyValuePairs
    |> Seq.groupBy (fun (KeyValue(key, _)) -> key)
    |> Seq.map (fun (key, values) -> key, values |> Seq.map (fun (KeyValue(_, value)) -> value) |> Seq.toList)
    |> Map.ofSeq

  let asyncReadAsString (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsStringAsync()

  let asyncReadAsStream (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsStreamAsync()

  let asyncReadAsBytes (Request reqMessage) =
    Async.AwaitTask <| reqMessage.Content.ReadAsByteArrayAsync()

  let asyncReadAsForm (Request reqMessage) = async {
    let! form = Async.AwaitTask <| reqMessage.Content.ReadAsAsync<FormDataCollection>()
    return toMap form }

  let asyncReadAs<'T> (Request reqMessage) = async {
    let formatters = reqMessage.GetConfiguration().Formatters
    let errors = ResizeArray()
    let logger = FormatterLogger(errors)
    let! result = Async.AwaitTask <| reqMessage.Content.ReadAsAsync<'T>(formatters, logger)
    let errors = Seq.toList errors
    return
      match errors with
      | [] -> Right result
      | head :: tail -> Left (head, tail) }

  let queryString (Request reqMessage) = 
    reqMessage.GetQueryNameValuePairs() |> toMap

  let routeValues (Request reqMessage) =
    let routeData = reqMessage.GetRouteData()
    routeData.Values
    |> Seq.choose (fun (KeyValue(key, value)) -> 
      if value = null then 
        None 
      else 
        Some (key, string value))
    |> Map.ofSeq
