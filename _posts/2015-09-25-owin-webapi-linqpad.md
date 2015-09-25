---
layout: post
title:  "OWIN WebAPI in LINQPad"
date:   2015-09-25
tags:   tech dotnet linqpad
---

[LINQPad](http://linqpad.net) is awesome. As a database query tools it's great, but it's a God-send as a prototyping tool. With NuGet integration and decent intellisense it's an ideal tool for rapidly developing sample code.

Of late I've been trying to get to grips with OWIN, the Open Web Interface for .NET. It's a fascinating move by Microsoft and I'm trying to lean how WebAPI fits into this structure, so out comes LINQPad.

In order to spin up a self-hosted Web API controller we need just one NuGet package and one System dll (below). We'll also grab a bit of WPF as well, that'll come in handy later.

```
Microsoft.AspNet.WebApi.OwinSelfHost (NuGet)
PresentationFramework.dll
System.Net.Http.dll
```

Using LINQPad's C# Program type, we can spin up a quick main method that will start a WebApp, fire up a browser and run for the lifetime of a Messsage box. This is a dead simple way to spin up a server that can be easily stopped again.

```csharp
void Main()
{
   using (var app = WebApp.Start<Startup>("http://localhost:8090"))
   {
      Process.Start("http://localhost:8090/api");
      MessageBox.Show("Dismiss this message to end the server");
   }
}
```

Our WebApp needs an OWIN Startup class. By convention this should have a _Configuration(...)_ method where we can set up the behaviour of our service. We need to do 3 things:

 * Provide a custom _IHttpControllerTypeResolver_
 * Set up our WebAPI routes
 * Tell OWIN we'd like to use WebAPI

```csharp
public class Startup
{
   public void Configuration(IAppBuilder app)
   {
      var config = new HttpConfiguration();
      config.Services.Replace(typeof(IHttpControllerTypeResolver), new ControllerResolver());
      config.MapHttpAttributeRoutes();

      app.UseWebApi(config);
   }
}
```

LINQPad takes our script, tops-and-tails it, and generates an assembly on the fly. So our custom Controller resolver will simply look at the executing assembly and return any _IHttpController_ implementations it finds.

```csharp
public class ControllerResolver : DefaultHttpControllerTypeResolver
{
   public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
   {
      return Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();
   }
}
```

And finally, our controller. I'm a fan of Attribute mapping but you can define your routes in the Startup class if you prefer.

```csharp
[RoutePrefix("api")]
public class DefaultController : ApiController
{
   [HttpGet, Route("")]
   public IHttpActionResult Get()
   {
      return Ok("Hello, world!");
   }
}
```

Download [Owin SelfHosted WebAPI.linq](/public/downloads/Owin SelfHosted WebAPI.linq)