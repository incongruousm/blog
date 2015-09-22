<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Microsoft.AspNet.WebApi.OwinSelfHost</NuGetReference>
  <Namespace>Microsoft.Owin</Namespace>
  <Namespace>Microsoft.Owin.Builder</Namespace>
  <Namespace>Microsoft.Owin.BuilderProperties</Namespace>
  <Namespace>Microsoft.Owin.Extensions</Namespace>
  <Namespace>Microsoft.Owin.Helpers</Namespace>
  <Namespace>Microsoft.Owin.Host.HttpListener</Namespace>
  <Namespace>Microsoft.Owin.Hosting</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Builder</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Engine</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Loader</Namespace>
  <Namespace>Microsoft.Owin.Hosting.ServerFactory</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Services</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Starter</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Tracing</Namespace>
  <Namespace>Microsoft.Owin.Hosting.Utilities</Namespace>
  <Namespace>Microsoft.Owin.Infrastructure</Namespace>
  <Namespace>Microsoft.Owin.Logging</Namespace>
  <Namespace>Microsoft.Owin.Mapping</Namespace>
  <Namespace>Microsoft.Owin.Security</Namespace>
  <Namespace>Owin</Namespace>
  <Namespace>System.Web.Http</Namespace>
  <Namespace>System.Web.Http.Controllers</Namespace>
  <Namespace>System.Web.Http.Dispatcher</Namespace>
  <Namespace>System.Windows</Namespace>
</Query>

void Main()
{
	using (var app = WebApp.Start<Startup>("http://localhost:8090"))
	{
		Process.Start("http://localhost:8090/api");
		MessageBox.Show("Dismiss this message to end the server");
	}
}

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

public class ControllerResolver : DefaultHttpControllerTypeResolver
{
	public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
	{
		return Assembly.GetExecutingAssembly()
					   .GetTypes()
					   .Where(i => typeof(IHttpController).IsAssignableFrom(i)).ToList();
	}
}

[RoutePrefix("api")]
public class DefaultController : ApiController
{
	[HttpGet, Route("")]
	public IHttpActionResult Get()
	{
		return Ok("Hello, world!");
	}
}