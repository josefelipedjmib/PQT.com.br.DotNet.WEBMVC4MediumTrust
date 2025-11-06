
using Essenciais.MVC.Areas;
using System.Web.Mvc;

namespace Modulo.Pagina.Areas.Pagina
{
    public class PaginaAreaRegistration : SmartAreaRegistration
    {
        public override string AreaName => "Pagina";

        protected override string[] GetNamespaces()
        {
            return new[] { "Modulo." + AreaName + ".Areas." + AreaName + ".Controllers" };
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            var url = "admin/" + AreaName + "/{action}/{pagina}";
            if (!JaRegistrada(url, AreaName))
            {
                // Rota padrão como fallback
                context.MapRoute(
                    AreaName + "AdminAction_default",
                    url,
                    new { controller = "Pagina", action = "Index", pagina = UrlParameter.Optional },
                    GetNamespaces()
                ).DataTokens["UseNamespaceFallback"] = false;
            }
        }
    }
}
