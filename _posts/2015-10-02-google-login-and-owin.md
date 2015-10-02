---
layout: post
title:  "Google Login and OWIN"
date:   2015-10-02
tags:   tech dotnet linqpad owin
---

In my adventures trying to get to grips with OWIN and WebAPI my next step was to look at how I can use Google as an external OAUTH login provider. My simple app will use Google as its sole form of login so I wanted to fire up my [LINQPad OWIN template]({% post_url 2015-09-25-owin-webapi-linqpad %}) and flesh out the extra steps required to prove a Google login.

Starting out with my template there's a couple of additional NuGet packages we're going to need.

```
Microsoft.Owin.Security.Cookies
Microsoft.Owin.Security.Google
```

I want to be able to prove a login and logout scenario and show that protected routes aren't available to unauthenticated users. So to start let's lock down our api controller with the `Authorize` attribute:

```csharp
[RoutePrefix("api"), Authorize]
public class DefaultController : ApiController
{
   \\ Snipped...
}
```

In order to kick-off authentication (and to sign out again) we'll need some form of account controller. To begin with just return a 200 for login and for logout, we'll fill these out later.

```csharp
[RoutePrefix("account")]
public class AccountController : ApiController
{
   [HttpGet, Route("login")]
   public IHttpActionResult Login(string returnUrl = null)
   {
      return Ok();
   }

   [HttpGet, Route("logout")]
   public IHttpActionResult Logout()
   {
      return Ok();
   }
}
```

It seems that the external login providers require cookie-based authentication in order to function. I had a bit of a play with other mechanisms for maintaining authentication state without success. Hopefully this will change in future versions but for now we need to tell the OWIN pipeline to use cookie authentication.

```csharp
var cookieOpts = new CookieAuthenticationOptions
{
   LoginPath = new PathString("/account/login"),
   CookieSecure = CookieSecureOption.SameAsRequest
};
app.UseCookieAuthentication(cookieOpts);
```

This code goes into the top of the OWIN Startup configuration method. `UseCookieAuthentication` adds OWIN middleware to the pipeline that monitors the response flow and transforms 401 responses from Web Api into 302 responses redirecting the user to the path given in `LoginPath`.

At this point we already have everything we need to redirect an unauthenticated access to `/api` to our login at `/account/login`. Next step is to configure a login workflow.

It's worth noting the `CookieSecure` property of `CookieAuthenticationOptions`, by default this is set to `SameAsRequest` so the cookie middleware will issue insecure cookies over HTTP. In a prod environment it's probably best to set this to `Always`.

In order to use Google for external login we need to add in the Google Authentication middleware. In it's simplest form this requires your Client Id and Client Secret from the [Google Developer Console](https://console.developers.google.com). This configuration needs to go into OWIN Startup after the cookie authentication configuration.

```csharp
app.SetDefaultSignInAsAuthenticationType(cookieOpts.AuthenticationType);
var googleOpts = new GoogleOAuth2AuthenticationOptions
{
   ClientId = "<my-client-id>",
   ClientSecret = "<my-client-secret>"
};
app.UseGoogleAuthentication(googleOpts);
```

Post-authentication Google will only redirect users back to one of a list of Redirect URLs specified in your Developer Console. By default the Google provider will specify a redirect URL of `/signin-google`. You can override this in `GoogleOAuth2AuthenticationOptions` but there's generally no need. The provider appears to internally register a route matching this path so a little bit of magic happens. :-)

The call to `SetDefaultSignInAsAuthenticationType` ensures that the Google login provider comes back with an Authentication Type that ties up to the cookie middleware, without this our login cookie won't be issued and there'll be no access for you (or anyone else).

We've now told OWIN to send unauthenticated requests to `/account/login` and that we'd like to use Google as a login provider but there still remains a step to kick the Google provider into action. In the real world you'd likely want a nice page informing your user they need to log in by clicking a pretty Google button, but for now we'll keep it much simpler and just modify our Account Controller.

```csharp
[HttpGet, Route("login")]
public IHttpActionResult Login(string returnUrl = null)
{
   var authProps = new AuthenticationProperties
   {
      RedirectUri = returnUrl
   };
   Request.GetOwinContext().Authentication.Challenge(authProps, "Google");
   return StatusCode(HttpStatusCode.Unauthorized);
}
```

The above login method tells the authentication middleware that it should challenge using the Google provider, we also return an HTTP 401. This kicks the server into initiating the Google OAuth login and sets us on our way. So the overall flow here is:

   1. User accesses `\api`
   2. asp.net returns an HTTP 401 due to the `Authorise` attribute
   3. Cookie authentication provider intercepts this and returns an HTTP 302 to `/account/login`
   4. The Account Controller adds a "Google" challenge and returns another HTTP 401
   5. Google authentication provider intercepts the challenge and initiates a Google login

The `returnUrl` parameter for `Login` matches the default value of `CookieAuthenticationOptions.ReturnUrlParameter`. The cookie middleware appends a parameter of this name to the query string when it makes a login redirect. The earlier magic of the `/signin-google` route will pick this up post-auth and send the user back to the URL they originally tried to access.

And we're almost done. To be tidy we can use our logoff action to clear down the access token granted in the OAuth exchange. Again, this is dead simple:

```csharp
[HttpGet, Route("logoff")]
public IHttpActionResult Logout()
{
   Request.GetOwinContext().Authentication.SignOut();
   return Ok();
}
```

Download [Owin SelfHosted WebAPI - Google Login.linq](/public/downloads/Owin SelfHosted WebAPI - Google Login.linq)