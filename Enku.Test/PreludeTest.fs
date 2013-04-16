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
  let ``ValidationContext.Eval should eval querystring value``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=10&name=hoge"))
    let qs = Request.getQueryStrings req
    let vc = ValidationContext()
    let id = vc.Eval(qs, "id", V.int <+> V.required)
    let name = vc.Eval(qs, "name", V.string <+> V.required)
    match vc.Message with
    | [] ->
      id.Value |> isEqualTo 10
      name.Value |> isEqualTo "hoge"
    | h :: _ -> 
      failwith h
    |> ignore

  [<Test>]
  let ``ValidationContext.Eval should eval querystring value and produce validation error messages``() =
    let req = Request <| new HttpRequestMessage(RequestUri = Uri("http://example/person?id=foo&name=hoge&age=bar"))
    let qs = Request.getQueryStrings req
    let vc = ValidationContext()
    let id = vc.Eval(qs, "id",V.int <+> V.required)
    let name = vc.Eval(qs, "name", V.string <+> V.required)
    let age = vc.Eval(qs, "age", V.int <+> V.required)
    match vc.Message with
    | [] -> failwith "validatioin should be fail"
    | messages -> printfn "%A" messages; List.length messages |> isEqualTo 2
    |> ignore

  type Person = { Name: string; Age: int }

  [<Test>]
  let ``ValidationContext.Eval should eval record properties``() =
    let person = { Name = "hoge"; Age = 30 }
    let vc = ValidationContext()
    vc.Eval(<@ person.Name @>, V.length 10) |> ignore
    vc.Eval(<@ person.Age @>, V.range 10 40) |> ignore
    match vc.Message with
    | [] ->
      ()
    | h :: _ -> 
      failwith h
    |> ignore

  [<Test>]
  let ``ValidationContext.Eval should eval record properties and produce validation error messages``() =
    let person = { Name = "hoge"; Age = 50 }
    let vc = ValidationContext()
    vc.Eval(<@ person.Name @>, V.length 2) |> ignore
    vc.Eval(<@ person.Age @>, V.range 10 40) |> ignore
    match vc.Message with
    | messages-> 
      List.length messages |> isEqualTo 2
      messages.[0] |> isEqualTo "Name is out of range (max=2)"
      messages.[1] |> isEqualTo "Age is out of range (min=10, max=40)"
    |> ignore