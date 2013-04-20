# Enku - F# Lightweight Web API Framework

Enku encourages you to build Web API in a F#-way.

- Simple
- Powerful
- Functional

## Install

To install Enku, run the following command in the Package Manager Console.

```
PM> Install-Package Enku
```

## Examples

### Self Host Example

Self Host Example requires SelfHost package.
To install SelfHost, run the following command in the Package Manager Console.

```
PM> Install-Package Microsoft.AspNet.WebApi.SelfHost
```

This example application listens to http://localhost:9090/. 
By default, listening at a particular HTTP address requires administrator privileges. 
When you run this application, therefore, you may get this error: 
"HTTP could not register URL http://+:9090/" 
There are two ways to avoid this error:

- Run Visual Studio with elevated administrator permissions, or
- Use Netsh.exe to give your account permissions to reserve the URL.

```fsharp
open System
open System.Web.Http
open System.Web.Http.SelfHost
open Enku

// configuration
let baseAddress = new Uri("http://localhost:9090/")
let config = 
  new HttpSelfHostConfiguration(
    baseAddress, 
    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always)
let route = Routing.route config

// routing
route "example" <| fun _ -> 
  [ 
    get, fun req -> async {
      return Response.Ok "Accept GET" }

    put <|> post, fun req -> async {
      let! content = Request.asyncReadAsString req
      return Response.Ok <| "Accept PUT or POST: content=" + content }

    any, fun req -> async {
      return Response.Ok "Accept any HTTP methods" }
  ], 
  fun req e ->
    Response.InternalServerError e

// run server
async {
  use server = new HttpSelfHostServer(config)
  do! Async.AwaitTask <| server.OpenAsync().ContinueWith(fun _ -> ()) 
  printfn "Server running at http://localhost:9090/"
  Console.ReadKey () |> ignore }
|> Async.RunSynchronously
```

Access

```
http://localhost:9090/example
```

### Web Host Example

Create a C# ASP.NET MVC 4 project with the Web API template.
Add a F# library project and then create a following module.

```fsharp
namespace Api

open Enku

module WebApiConfig =

  [<CompiledNameAttribute("Register")>]
  let register config =
    let route = Routing.route config

    // routing
    route "example" <| fun _ -> 
      [ 
        get, fun req -> async {
          return Response.Ok "Accept GET" }

        put <|> post, fun req -> async {
          let! content = Request.asyncReadAsString req
          return Response.Ok <| "Accept PUT or POST: content=" + content }

        any, fun req -> async {
          return Response.Ok "Accept any HTTP methods" }
      ], 
      fun req e ->
        Response.InternalServerError e
```

In Global.asax, use the ablove WebApiConfig module instead of the original WebApiConfig class.

```csharp
WebApiConfig.Register(GlobalConfiguration.Configuration);
```

## Resources

- [Web API in a F#-way](http://www.rvl.io/nakamura_to/web-api-in-a-fsharp-way)

## License

Apache License, Version 2.0

http://www.apache.org/licenses/LICENSE-2.0.html
