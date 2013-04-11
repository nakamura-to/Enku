namespace Enku.Test

open NUnit.Framework
open System
open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open System.Text.RegularExpressions
open Enku

module RequestTest = 

  module V = Validator

  [<Test>]
  let ``Request.validate should eval lazy values``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=10&name=hoge"))
    let qs = req.QueryString
    let id = qs.Value "id" (V.head + V.int + V.required)
    let name = qs.Value "name" (V.head + V.string + V.required)
    req.Validate <| function
        | [] ->
          id.Value |> isEqualTo 10
          name.Value |> isEqualTo "hoge"
        | h :: _ -> failwith h
    |> ignore

  [<Test>]
  let ``Request.validate should produce validation error messages``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=foo&name=hoge&age=bar"))
    let qs = req.QueryString
    let id = qs.Value "id" (V.head + V.int + V.required)
    let name = qs.Value "name" (V.head + V.string + V.required)
    let age = qs.Value "age" (V.head + V.int + V.required)
    req.Validate <| function
      | [] -> failwith "validatioin should be fail"
      | messages -> List.length messages |> isEqualTo 2
    |> ignore
