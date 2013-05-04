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
open Newtonsoft.Json.Serialization
open Enku

// configuration
let config = new HttpSelfHostConfiguration("http://localhost:9090/")
config.IncludeErrorDetailPolicy <- IncludeErrorDetailPolicy.Always
config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
let route = Routing.route config

// routing
route "example" <| fun _ -> 
  [
    get, fun req -> async {
      return Content.json "Accept GET" |> Response.Ok }

    put <|> post, fun req -> async {
      let! content = Request.asyncReadAsString req
      return Content.json ("Accept PUT or POST: content=" + content) |> Response.Ok }

    any, fun req -> async {
      return Content.json "Accept any HTTP methods" |> Response.Ok }
  ], 
  fun req e -> Content.error e |> Response.InternalServerError

// run server
let server = new HttpSelfHostServer(config)
server.OpenAsync().Wait()
printfn "Server running at http://localhost:9090/"
Console.ReadKey () |> ignore
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

open System.Web.Http
open Newtonsoft.Json.Serialization
open Enku

module WebApiConfig =

  [<CompiledNameAttribute("Register")>]
  let register (config: HttpConfiguration) =
    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()
    let route = Routing.route config

    // routing
    route "example" <| fun _ -> 
      [
        get, fun req -> async {
          return Content.json "Accept GET" |> Response.Ok }

        put <|> post, fun req -> async {
          let! content = Request.asyncReadAsString req
          return Content.json ("Accept PUT or POST: content=" + content) |> Response.Ok }

        any, fun req -> async {
          return Content.json "Accept any HTTP methods" |> Response.Ok }
      ], 
      fun req e -> Content.error e |> Response.InternalServerError
```

In Global.asax, use the above WebApiConfig module instead of the original WebApiConfig class.

```csharp
WebApiConfig.Register(GlobalConfiguration.Configuration);
```

## License

Apache License, Version 2.0

http://www.apache.org/licenses/LICENSE-2.0.html
