using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Essenciais.MVC.Extensions;

namespace Essenciais.MVC.ActionFilters
{
    public class AtivadoConfigActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var ativo = false;
            
            var routerData = filterContext.RouteData;
            bool.TryParse(ConfigurationManager.AppSettings["Ativado:" + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.CapitalizeFirstLetter() + filterContext.ActionDescriptor.ActionName.CapitalizeFirstLetter()], out ativo);
            if (ativo)
                return;
            var urlSair = URLsPadrao.Sair.Split('/');
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                (
                    new
                    {
                        controller = urlSair[1],
                        action = urlSair[2],
                        area = "Admin" //Rota padrão para deslogar
                    }
                )
            );
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var routeData = filterContext.RouteData;
        }
    }
}