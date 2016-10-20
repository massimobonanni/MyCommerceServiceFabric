using System.Reflection;
using System.Web.Http;
using Owin;
using WebApi.Controllers;

namespace WebApi
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            var container = UnityConfig.RegisterComponents(config);
            config.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }
    }
}
