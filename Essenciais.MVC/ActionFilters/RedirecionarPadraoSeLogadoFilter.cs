using System.Web.Mvc;
using Essenciais.MVC.Extensions;

namespace Essenciais.MVC.ActionFilters
{
    public class RedirecionarPadraoSeLogadoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session.IsLoginValid())
                filterContext.HttpContext.Response.Redirect(URLsPadrao.Painel);
        }
    }
}