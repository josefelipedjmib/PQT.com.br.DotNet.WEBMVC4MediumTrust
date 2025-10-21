using Essenciais.MVC.Extensions;
using Service;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Mvc = System.Web.Mvc;

namespace Essenciais.MVC.Services
{
    public static class ActionFilterService
    {
        // Para MVC
        public static bool TemAllowAnonymous(Mvc.ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor;
            var controllerDescriptor = actionDescriptor.ControllerDescriptor;

            return TemAtributoAllowAnonymous(actionDescriptor)
                || TemAtributoAllowAnonymous(controllerDescriptor);
        }

        // Para Web API
        public static bool TemAllowAnonymous(HttpActionContext context)
        {
            var actionDescriptor = context.ActionDescriptor;
            var controllerDescriptor = context.ControllerContext.ControllerDescriptor;

            return TemAtributoAllowAnonymous(actionDescriptor)
                || TemAtributoAllowAnonymous(controllerDescriptor);
        }

        // Método genérico para MVC
        private static bool TemAtributoAllowAnonymous(ICustomAttributeProvider descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(Mvc.AllowAnonymousAttribute), true).Any();
        }

        // Método genérico para Web API
        private static bool TemAtributoAllowAnonymous(HttpActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        private static bool TemAtributoAllowAnonymous(HttpControllerDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public static bool TemLoginValido(object context)
        {
            try
            {
                HttpSessionStateBase session = null;

                if (context is Mvc.ActionExecutingContext mvcContext)
                {
                    session = mvcContext.HttpContext.Session;
                }
                else if (HttpContext.Current != null)
                {
                    session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                }

                if (session != null && session.IsLoginValid())
                {
                    if (session.GetLogin().Equals("000001"))
                        return true;
                    var usuarioService = new UsuarioService();
                    var usuario = usuarioService.Listar(session.GetLogin(), "id", 1).Data?.FirstOrDefault();
                    return usuario != null;
                }
            }catch(Exception ex)
            {

            }

            return false;
        }
    }
}
