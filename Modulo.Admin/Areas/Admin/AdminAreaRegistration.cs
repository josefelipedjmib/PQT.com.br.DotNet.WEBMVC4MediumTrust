
using Essenciais.MVC.Areas;
using Essenciais.MVC.Routes;
using System.Web.Mvc;

namespace Modulo.Admin.Areas.Admin
{
    public class AdminAreaRegistration : SmartAreaRegistration
    {
        public override string AreaName => "Admin";

        protected override string[] GetNamespaces()
        {
            return new[] { "Modulo." + AreaName + ".Areas." + AreaName + ".Controllers" };
        }
    }
}