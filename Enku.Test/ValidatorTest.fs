namespace Enku.Test

open NUnit.Framework
open System
open System.Net.Http
open System.Web.Http
open System.Web.Http.Hosting
open System.Text.RegularExpressions
open Enku
open Enku.Validation

module ValidatorTest = 

  [<Test>]
  let ``Validator.head should return a first value``() =
    let (Validator validator) = Validator.head
    match validator "x" (Some [1..3]) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 1
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.head should recognize an empty list as invalid``() =
    let (Validator validator) = Validator.head
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not found."

  [<Test>]
  let ``Validator.headWith should accept a customized message``() =
    let (Validator validator) = Validator.headWith "[%s]"
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.int should convert a value to a int value``() =
    let (Validator validator) = Validator.int
    match validator "x" (Some "10") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 10
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.int should recognize a character as invalid``() =
    let (Validator validator) = Validator.int
    match validator "x" (Some "a") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not a int."

  [<Test>]
  let ``Validator.intWith should accept a customized error message``() =
    let (Validator validator) = Validator.intWith "[%s]"
    match validator "x" (Some []) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.maxlength should check string length``() =
    let (Validator validator) = Validator.maxlength 5
    match validator "x" (Some "abcde") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "abcde"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.maxlength should recognize a greater value as invalid``() =
    let (Validator validator) = Validator.maxlength 5
    match validator "x" (Some "abcdef") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x can not be greater than 5 characters."

  [<Test>]
  let ``Validator.maxlength should accept a customized error message``() =
    let (Validator validator) = Validator.maxlengthWith 5 "[%s %d]" 
    match validator "x" (Some "abcdef") with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x 5]"

  [<Test>]
  let ``Validator.range should check value range``() =
    let (Validator validator) = Validator.range 5M 15M
    match validator "x" (Some 5M) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 5M
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.range should recognize a out-of-range value as invalid``() =
    let (Validator validator) = Validator.range 5M 15M
    match validator "x" (Some 4M) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is not in the range 5M through 15M."

  [<Test>]
  let ``Validator.range should accept a customized error message``() =
    let (Validator validator) = Validator.rangeWith 5M 15M "[%s %A %A]" 
    match validator "x" (Some 4M) with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x 5M 15M]"

  [<Test>]
  let ``Validator.required should require a value``() =
    let (Validator validator) = Validator.required
    match validator "x" (Some "abcde") with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "abcde"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.range should recognize none as invalid``() =
    let (Validator validator) = Validator.required
    match validator "x" None with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "x is required."

  [<Test>]
  let ``Validator.required should accept a customized error message``() =
    let (Validator validator) = Validator.requiredWith "[%s]" 
    match validator "x" None with
    | Result.Ok r -> fail()
    | Result.Error msg -> msg |> isEqualTo "[x]"

  [<Test>]
  let ``Validator.string should strigify a value``() =
    let (Validator validator) = Validator.string
    match validator "x" (Some 123) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo "123"
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``Validator.string should strigify a null value``() =
    let (Validator validator) = Validator.string
    match validator "x" (Some null) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo ""
      | _ -> fail()
    | _ -> fail()

  [<Test>]
  let ``validator should skip None value``() =
    let (Validator validator) = Validator.head
    match validator "x" None with
    | Result.Ok r -> r |> isEqualTo None
    | _ -> fail()

  [<Test>]
  let ``validator should be composable``() =
    let (Validator validator) = Validator.head <+> Validator.int <+> Validator.range 10 20
    match validator "x" (Some ["15"; "30"]) with
    | Result.Ok r -> 
      match r with
      | Some r -> r |> isEqualTo 15
      | _ -> fail()
    | _ -> fail()