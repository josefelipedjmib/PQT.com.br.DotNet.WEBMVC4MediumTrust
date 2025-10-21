using System.Web.Mvc;
using Domain.Models;

namespace Web.MVC.Controllers
{
    public class HomeController : Controller
    {
        //Index - seria uma ação padrão, caso não tenha,
        //content/ctd/principal.xml - será usado esse arquivo.
        public ActionResult ExemploDeUmaAcaoACustomizar()
        {
            return View(new Retorno<string>
            {
                Mensagem = "Funcionou",
                Data = "Chegou aqui!"
            });
        }
    }
}