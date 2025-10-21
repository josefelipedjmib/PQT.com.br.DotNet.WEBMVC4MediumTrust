using System.Web;
using System.Web.Routing;

namespace Web.MVC.Handlers
{
    public class SessionRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routeData = requestContext.RouteData;
            return new SessionEnabledControllerHandler(routeData);
        }
    }

}