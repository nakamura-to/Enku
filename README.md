# Enku - F# Lightweight Web API Framework

Enku provides Web API in a F#-way.

- Simple
- Powerful
- Functional

```fsharp
route "example/{?id}" <| fun _ -> 
  [ 
    get, fun req -> async {
      return Response.OK "Accept GET" }

    put <|> post, fun req -> async {
      return Response.OK "Accept PUT or POST" }

    delete, fun req -> async {
      return Response.OK "Accept DELETE" }

    any, fun req -> async {
      return Response.OK "Accept any HTTP methods" }
  ], 
  fun req e ->
    Response.InternalServerError e
```

## Install

N/A

## Examples

N/A

## Quick Start

N/A

## License

Apache License, Version 2.0

http://www.apache.org/licenses/LICENSE-2.0.html