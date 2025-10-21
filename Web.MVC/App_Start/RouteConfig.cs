using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebSockets;

namespace Web.MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //Descomentar para poder usar a página Index.html como padrão.
            //routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(html|htm)" });

            // Ignora chamadas para recursos internos do ASP.NET
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ignora qualquer rota com inicial de nome de Módulo
            var modulos = new string[]
            {
                "admin",
                //Adicione os módulos aqui.
            };
            foreach (var modulo in modulos)
            {
                routes.IgnoreRoute($"{modulo.ToLower()}/{{*pathInfo}}");
            }

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Web.MVC.Controllers" }
            ).DataTokens["UseNamespaceFallback"] = false;
        }
    }
}
