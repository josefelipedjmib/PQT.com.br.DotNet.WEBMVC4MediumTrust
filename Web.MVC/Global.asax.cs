using Service;
using System.Globalization;
using System.Threading;
using System.Web;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.MVC.App_Start;
using System.Web.Http.WebHost;
using Web.MVC.Handlers;
using Auxiliar.Helper;
using Essenciais.MVC.Extensions;

namespace Web.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Registra manualmente a área AREA primeiro
            //var area = new Modulo.__AREA__.Areas.__AREA__.__AREA__AreaRegistration();
            //var context = new AreaRegistrationContext(area.AreaName, RouteTable.Routes);
            //area.RegisterArea(context);

            var areaPagina = new Modulo.Pagina.Areas.Pagina.PaginaAreaRegistration();
            var context = new AreaRegistrationContext(areaPagina.AreaName, RouteTable.Routes);
            areaPagina.RegisterArea(context);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            foreach (var routeBase in RouteTable.Routes)
            {
                var route = routeBase as Route;
                if (route != null && route.RouteHandler is HttpControllerRouteHandler)
                {
                    route.RouteHandler = new SessionRouteHandler();
                }
            }
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpApplication httpApp = this.Context.ApplicationInstance;
            HttpException httpEx = httpApp.Server.GetLastError() as HttpException;
            var erroMensagem = "";

            if (httpEx != null)
            {
                if (httpEx.GetHttpCode().Equals(404))
                    return;

                if (
                    httpEx.WebEventCode == System.Web.Management.WebEventCodes.AuditInvalidViewStateFailure
                    ||
                    httpEx.WebEventCode == System.Web.Management.WebEventCodes.InvalidViewState
                    ||
                    httpEx.WebEventCode == System.Web.Management.WebEventCodes.InvalidViewStateMac
                    ||
                    httpEx.WebEventCode == System.Web.Management.WebEventCodes.RuntimeErrorViewStateFailure
                    )
                {
                    HttpContext.Current.ClearError();
                    erroMensagem = "Error: An invalid viewstate has been detected (WebEventCode: " + httpEx.WebEventCode.ToString() + ").";
                }
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
            Exception ex = Server.GetLastError();
            if (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            var usuario = "";
            var session = HttpContext.Current.Session;
            if (session.IsLoginValid())
            {
                usuario = session.GetLogin();
            }
            var path = Request.Url.PathAndQuery;
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);

            var configs = ConfigurationsManagerHelper.GetAllAppSettings(ConfigurationsManagerHelper.GetAppSettingsAsDictionary, null);
            Infrastructure.LogSystem.Prepare(configs);
            Infrastructure.LogSystem.Logar(usuario, "500", path, "" + ex, urlHelper.GetIPTratado(), false);
        }
    }
}
