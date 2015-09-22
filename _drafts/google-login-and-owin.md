#Google Login and OWIN
- Jason Deabill
- incongruousm
- 2015-07-20
- Tech
- draft

In my adventures trying to get to grips with OWIN and WebAPI my next step is to look at Google as an external OAUTH login provider. My simple app will use Google as its sole form of login so I wanted to fire up my LINQPad and flesh out the extra steps required to prove a Google login.

Starting out with my simple [LINQPad self-hosted app](owin-webapi-linqpad) the first step is to configure cookie based authentication. For that, we need a couple of additional NuGet packages.

```
Microsoft.Owin.Security.Cookies
Microsoft.Owin.Security.Google
```

```cs
var cookieOpts = new CookieAuthenticationOptions
{
	LoginPath = new PathString("/account/login")
};
app.UseCookieAuthentication(cookieOpts);
```

This code goes into the top of the OWIN Startup configuration method. `UseCookieAuthentication`will cause any unauthenticated requests to controllers/actions marked `Authorize` to be redirected (HTTP 302) to the path given in `LoginPath`. We can then create an Account controller to trigger a login workflow. It's worth noting the `CookieSecure` property of `CookieAuthenticationOptions`, by default this is set to `SameAsRequest` so the cookie middleware will issue insecure cookies over HTTP. In a prod environment it's probably best to set this to `Always`.

```cs
app.SetDefaultSignInAsAuthenticationType(cookieOpts.AuthenticationType);
var googleOpts = new GoogleOAuth2AuthenticationOptions
{
	ClientId = "<my-client-id>",
	ClientSecret = "<my-client-secret>"
};
app.UseGoogleAuthentication(googleOpts);
```

Once the cookie middleware is set up we can add in the configuration for the Google OAuth provider. In it's simplest form this requires your Client Id and Client Secret from the [Google Developer Console](https://console.developers.google.com). Post-authentication Google will only redirect users back to one of a list of Redirect URLs specified in your Developer Console. By default the Google provider will specify a redirect URL of `/signin-google`. You can override this in `GoogleOAuth2AuthenticationOptions` but there may be no need. The provider appears to internally register a route matching this path so a little bit of magic happens. :-)

The call to `SetDefaultSignInAsAuthenticationType` ensures the the Google provider comes back with an Authentication Type that ties up to the cookie middleware, without this our login cookie won't be issued and there'll be no access for you (or anyone else).

The final step is to create our Account controller to deal with those login redirects. In the real world you'd likely want a nice page informing your user they need to log in by clicking this pretty Google button, but for now we'll keep it much simpler.

```cs
[RoutePrefix("account")]
public class AccountController : ApiController
{
	[HttpGet, Route("login")]
	public IHttpActionResult Login(string returnUrl)
	{
		var authProps = new AuthenticationProperties
		{
			RedirectUri = returnUrl
		};
		Request.GetOwinContext().Authentication.Challenge(authProps, "Google");
		return StatusCode(HttpStatusCode.Unauthorized);
	}
}
```

The above login method tells the authentication middleware that it should challenge using the Google provider, we also return an HTTP 401. This kicks the server into initiating the Google OAuth login and sets us on our way.

The `returnUrl` parameter matches the default value of `CookieAuthenticationOptions.ReturnUrlParameter`. The cookie middleware appends a parameter of this name to the query string when it makes a login redirect. The earlier magic of the `/signin-google` route will pick this up post-auth and send the user back to the URL they originally tried to access.

You may wish to add a logoff action to clear down the access token granted in the OAuth exchange. Again, this is dead simple:
```cs
[HttpGet, Route("logoff")]
public IHttpActionResult Logout()
{
	Request.GetOwinContext().Authentication.SignOut();
	return Ok();
}
```