namespace Enku.Test

open NUnit.Framework
open System
open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open System.Text.RegularExpressions
open Enku

module ValidationTest = 

  [<Test>]
  let ``fromQuery should return query string``() =
    use req = new HttpRequestMessage(RequestUri = Uri("http://example/person?id=10"))
    validation { 
      return! "id" |> Request.fromQuery req (Validator.first >>> Validator.int) }
    |> function
    | Success ret -> ret |> isEqualTo 10
    | Failure msg -> failwith msg

  [<Test>]
  let ``fromQuery should not return query string``() =
    use req = new HttpRequestMessage(RequestUri = Uri("http://example/person?id=abc"))
    validation { 
      return! "id" |> Request.fromQuery req (Validator.first >>> Validator.int) }
    |> function
    | Success ret -> failwith ""
    | Failure msg -> msg |> isEqualTo "id is not int"

