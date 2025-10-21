using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.Mvc;

namespace Essenciais.MVC.Routes
{
    public class CustomSmartRoute : RouteBase
    {
        private readonly string _fixedController;
        private readonly string[] _namespaces;
        private readonly string _area;

        public CustomSmartRoute(string fixedController, string[] namespaces, string area = "")
        {
            _fixedController = fixedController;
            _namespaces = namespaces.Select(ns => ns.Trim()).ToArray(); ;
            _area = area;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var path = httpContext.Request.AppRelativeCurrentExecutionFilePath.TrimStart('~', '/');
            var segments = path.Split('/');
            if (segments.Length > 1 && string.IsNullOrEmpty(segments[segments.Length - 1]))
            {
                segments = segments.Take(segments.Length - 1).ToArray();
            }

            if (segments.Length < 2 || !segments[0].Equals(_fixedController, StringComparison.OrdinalIgnoreCase))
                return null;

            var routeData = new RouteData(this, new MvcRouteHandler());

            string candidateController = _fixedController.Trim();
            string candidateAction = segments[1].Trim();

            var controllerType = GetControllerType(candidateController);

            if (controllerType != null && HasAction(controllerType, candidateAction))
            {
                // Usa controller fixo com a action especificada
                routeData.Values["controller"] = candidateController;
                routeData.Values["action"] = candidateAction;
                routeData.DataTokens["area"] = _area;
                routeData.DataTokens["Namespaces"] = _namespaces;
                // Suporte opcional para id
                if (segments.Length > 2)
                {
                    routeData.Values["id"] = segments[2];
                }
                else
                {
                    routeData.Values["id"] = UrlParameter.Optional;
                }
                return routeData;
            }
            ////Verifica se candidateAction é uma controller válida na área
            //controllerType = GetControllerType(candidateAction);
            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null; // Não necessário para este caso
        }

        private Type GetControllerType(string controllerName)
        {
            var fullControllerName = controllerName + "Controller";
            var assemblies = BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(t =>
                    typeof(IController).IsAssignableFrom(t) &&
                    t.Name.Equals(fullControllerName, StringComparison.OrdinalIgnoreCase) &&
                    t.Namespace != null
                ).ToList();
            Type controller;
            if (!string.IsNullOrEmpty(_area))
                controller = assemblies.FirstOrDefault(t =>
                     _namespaces.Any(ns => t.Namespace.Equals(ns, StringComparison.OrdinalIgnoreCase)));
            else
                controller = assemblies.FirstOrDefault(t =>
                _namespaces.Any(ns => t.Namespace.Equals(ns)));
            return controller;
        }

        private bool HasAction(Type controllerType, string actionName)
        {
            return controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Any(m => m.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));
        }
    }
}