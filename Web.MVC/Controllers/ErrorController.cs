using Auxiliar.Helper;
using System;
using System.Net;
using System.Web.Mvc;
using Essenciais.MVC.Extensions;

namespace Web.MVC.Controllers
{
    public class ErrorController : Controller
    {
        private string _paginaPadrao = "principal";

        [ActionName("404")]
        public ActionResult Error404()
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress, _paginaPadrao);
            var paginaArquivo = urlHelper.GetPaginaArquivo(urlHelper.Pagina);
            if(!IOHelper.TemExtensao(paginaArquivo))
            {
                var arquivoXML = IOHelper.GetPathFileXMLPadrao(paginaArquivo);
                var conteudo = IOHelper.GetTextNodeXML(arquivoXML);
                if (!string.IsNullOrEmpty(conteudo))
                    return View("ConteudoXML", "", conteudo);

                var usuario = "";
                if (Session.IsLoginValid())
                {
                    usuario = Session.GetLogin();
                }
                var path = Request.Url.PathAndQuery;

                var configs = ConfigurationsManagerHelper.GetAllAppSettings(ConfigurationsManagerHelper.GetAppSettingsAsDictionary, null);
                Infrastructure.LogSystem.Prepare(configs);
                Infrastructure.LogSystem.Logar(usuario, ((int)HttpStatusCode.NotFound).ToString(), path, "página não encontrada", urlHelper.GetIPTratado(), false, false);
            }
            var status = new HttpStatusCodeResult(HttpStatusCode.NotFound, "404 Not Found");
            Response.Status = status.StatusDescription;
            Response.StatusCode = status.StatusCode;
            Response.StatusDescription = status.StatusDescription;
            return View();
        }

        [ActionName("500")]
        public ActionResult Error500()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }

            var status = new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "500 Internal Server Error");
            Response.Status = status.StatusDescription;
            Response.StatusCode = status.StatusCode;
            Response.StatusDescription = status.StatusDescription;
            return View();
        }
    }
}