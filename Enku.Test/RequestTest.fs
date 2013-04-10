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
    use req = new HttpRequestMessage(RequestUri = Uri("http://example/person?id=10&name=hoge"))
    get { 
      let! id = Request.queryString "id" (V.head + V.int + V.required)
      let! name = Request.queryString "name" (V.head + V.string + V.required)
      do! Request.validate <| function
        | [] ->
          id.Value |> isEqualTo 10
          name.Value |> isEqualTo "hoge"
        | h :: _ -> failwith h }
    |> Action.run req
    |> function
    | Completion _ -> ()
    | _ -> failwith "fail"

  [<Test>]
  let ``Request.validate should produce validation error messages``() =
    use req = new HttpRequestMessage(RequestUri = Uri("http://example/person?id=foo&name=hoge&age=bar"))
    get { 
      let! id = Request.queryString "id" (V.head + V.int + V.required)
      let! name = Request.queryString "name" (V.head + V.string + V.required)
      let! age = Request.queryString "age" (V.head + V.int + V.required)
      do! Request.validate <| function
        | [] -> failwith "validatioin should be fail"
        | messages -> List.length messages |> isEqualTo 2}
    |> Action.run req
    |> function
    | Completion _ -> ()
    | _ -> failwith "fail"
