using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Essenciais.MVC.Services;

namespace Essenciais.MVC.ActionFilters
{
    public class AutorizadorActionFilter : ActionFilterAttribute
    {
        private bool _useACL = false;

        public AutorizadorActionFilter()
        {
            var useACL = ConfigurationManager.AppSettings["useACL"];
            bool.TryParse(useACL, out _useACL);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (ActionFilterService.TemAllowAnonymous(filterContext)) return;
            if (ActionFilterService.TemLoginValido(filterContext))
            {
                if (!_useACL)
                    return;
            }
            var routerData = filterContext.RouteData;
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