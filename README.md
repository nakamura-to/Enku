# Enku - F# Lightweight Web API Framework

Enku provides Web API in a F#-way.

- Simple
- Powerful
- Functional

```fsharp
route "example/{?id}" <| fun req -> 
  [ 
    get
      { return "Accept GET" }
    (put <|> post)
      { return "Accept PUT or POST"}
    delete
      { return async 
          { return "Accept DELETE and process asynchronously" } }
    any
      { return "Accept Any HTTP Methods" }
  ]
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