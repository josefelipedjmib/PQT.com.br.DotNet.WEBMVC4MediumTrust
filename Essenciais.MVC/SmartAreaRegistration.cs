using Essenciais.MVC.Routes;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Essenciais.MVC.Areas
{
    public abstract class SmartAreaRegistration : AreaRegistration
    {
        public abstract override string AreaName { get; }

        protected abstract string[] GetNamespaces();

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Rota inteligente
            context.Routes.Add(AreaName + "SmartRoute", new CustomSmartRoute(
                AreaName,
                GetNamespaces(),
                AreaName
            ));

            var url = AreaName + "/{controller}/{action}/{id}";
            if (!JaRegistrada(url, AreaName))
            {
                // Rota padrão como fallback
                context.MapRoute(
                    AreaName + "_default",
                    url,
                    new { controller = AreaName, action = "Index", id = UrlParameter.Optional },
                    GetNamespaces()
                ).DataTokens["UseNamespaceFallback"] = false;
            }
        }

        public bool JaRegistrada(string url, string area)
        {
            return RouteTable.Routes
                .OfType<Route>()
                .Any(r => r.DataTokens != null &&
                          r.DataTokens.ContainsKey("area") &&
                          r.DataTokens["area"].ToString().Equals(area, StringComparison.OrdinalIgnoreCase) &&
                          r.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
        }
    }
}