using Aptera.Sitefinity.Decorators;
using SitefinityWebApp.Custom.AlbumOptimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Telerik.Sitefinity;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Libraries.ImageProcessing;
using Telerik.Sitefinity.Libraries.Model;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {
        public static Dictionary<string, Guid> KrakenCallbackIds = new Dictionary<string, Guid>();

        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Initialized += new EventHandler<ExecutedEventArgs>(Bootstrapper_Initialized);
        }

        protected void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                RegisterRoutes(RouteTable.Routes);

			 ObjectFactory.Container.RegisterType<ILifecycleDecorator, DocumentsDecorator>(typeof(LibrariesManager).FullName,
			    new InjectionConstructor(
				   new InjectionParameter<ILifecycleManager>(null),
				   new InjectionParameter<Action<Content, Content>>(null),
				   new InjectionParameter<Type[]>(null)));

			 Config.RegisterSection<AlbumOptimizationConfig>();

			 App.WorkWith()
				.DynamicData()
					.Type(typeof(Image))
						.Field()
							 .TryCreateNew("Optimized", typeof(bool))
							 .SaveChanges(true);
            }
        }

        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}