using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace Web.MVC.Handlers
{
    public class SessionEnabledControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionEnabledControllerHandler(RouteData routeData) : base(routeData) { }
    }

}