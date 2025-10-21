using Essenciais.MVC.Services;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Essenciais.MVC.ActionFilters
{
    public class AutorizadorAPIActionFilter : ActionFilterAttribute
    {
        private bool _useACL = false;

        public AutorizadorAPIActionFilter()
        {
            var useACL = ConfigurationManager.AppSettings["useACL"];
            bool.TryParse(useACL, out _useACL);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            if (ActionFilterService.TemAllowAnonymous(actionContext)) return;
            if (ActionFilterService.TemLoginValido(actionContext))
            {
                if (!_useACL)
                    return;
            }

            var controller = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var action = actionContext.ActionDescriptor.ActionName;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var action = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            // Sua lógica após a execução da ação
        }
    }
}