using System.Web.Mvc;
using Essenciais.MVC.ActionFilters;
using Essenciais.MVC.Extensions;
using System.Threading.Tasks;
using Auxiliar.Helper;
using System.Linq;
using System.IO;
using Essenciais.MVC.Controllers;
using Domain.Models;
using System.Collections.Generic;

namespace Modulo.Pagina.Areas.Pagina.Controllers
{
    [AutorizadorActionFilter]
    public class PaginaController : PaginatedController
    {
        private Dictionary<string, string> _configs = ConfigurationsManagerHelper.GetAllAppSettings(ConfigurationsManagerHelper.GetAppSettingsAsDictionary, null);
        private string PaginaPadrao = "Index";
        // GET: Pagina/Action
        [PaginadaOrdenadaActionFilter]
        public ActionResult Index()
        {
            var mensagem = "";
            if (TempData["Mensagem"] != null)
                mensagem += TempData["Mensagem"].ToString();
            var arquivosApagados = IOHelper.ApagarXMLsDeletadosExpirados();
            if (arquivosApagados > 0)
                mensagem += "<br> Foram apagados " + arquivosApagados + " arquivo(s) expirado(s).";
            ViewBag.Mensagem = mensagem;
            Retorno.Cabecalhos = new List<PropriedadeConfig>
                {
                    new PropriedadeConfig { NomePropriedade = "Nome", NomeExibicao = "Nome" },
                    new PropriedadeConfig { NomePropriedade = "Data", NomeExibicao = "Data" },
                    new PropriedadeConfig { NomePropriedade = "Arquivo", NomeExibicao = "Caminho completo"}
                };
            Retorno.TotalItens = IOHelper.GetTotalXMLConteudoFiltered();
            if (Retorno.TotalItens < 1)
                Retorno.Errors.Add("Nenhum dado retornado.");
            else
            {
                var arquivos = IOHelper.GetAllXMLConteudosPadrao(Retorno.PaginasNumero, Retorno.PaginasTamanho, Retorno.OrdenacaoAtual).Data;
                Retorno.Data = arquivos.Select(arquivo =>
                {
                    var dict = Retorno.TratarDict(arquivo);
                    // Adiciona o Id ou chave para os links de ação
                    dict["Id"] = arquivo.Nome;
                    return dict;
                }).ToList();
            }
            Retorno.RootPath = "/admin/pagina/";
            return View(Retorno);
        }

        public ActionResult Detalhar(string pagina)
        {
            if (string.IsNullOrEmpty(pagina))
                return RedirectToAction(PaginaPadrao);
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(pagina);
            var conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo, IOHelper.XMLDeletedArquivosMarca });
        }

        public ActionResult Lixeira(string pagina)
        {
            var arquivosXml = IOHelper.GetDeletedsXMLConteudosPadrao();
            return View(arquivosXml);
        }

        public ActionResult Novo()
        {
            return View(new string[] { "", "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Novo(string pagina, string htmlEditor)
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var erro = IOHelper.NovoXMLConteudoPadrao(pagina, htmlEditor);
            if (string.IsNullOrEmpty(erro))
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Pagina - novo", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Cadastro de página: " + pagina + " - erros: " + erro, urlHelper.GetIPTratado(), string.IsNullOrEmpty(erro));
                TempData["Mensagem"] = @"
                                Arquivo criado com sucesso.
                            ";
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Pagina - novo", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Cadastro de página: " + pagina + " - erros: " + erro, urlHelper.GetIPTratado(), string.IsNullOrEmpty(erro));
            ViewBag.Mensagem = erro;
            return View(new string[] { pagina, htmlEditor });
        }

        public ActionResult Editar(string pagina)
        {
            if (string.IsNullOrEmpty(pagina))
                return RedirectToAction(PaginaPadrao);
            ViewBag.Historicos = IOHelper.GetAllPaginaFilesWithHistory(pagina)
                                    .Select(
                                        arquivo => 
                                            Path.GetFileName(arquivo).Replace(IOHelper.XMLExtensao, "")
                                    ).ToArray();
            var historico = Request.QueryString["historico"];
            var arquivoEscolhido = pagina;
            if (!string.IsNullOrEmpty(historico))
                arquivoEscolhido = historico;
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(arquivoEscolhido);
            var conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Editar(string pagina, string htmlEditor)
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var erro = IOHelper.EditXMLConteudoPadrao(pagina, htmlEditor);
            if (string.IsNullOrEmpty(erro))
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Pagina - editar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Alteração de página: " + pagina + " - erros: " + erro, urlHelper.GetIPTratado(), string.IsNullOrEmpty(erro));
                TempData["Mensagem"] = @"
                                Arquivo alterado com sucesso.
                            ";
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Pagina - editar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Alteração de página: " + pagina + " - erros: " + erro, urlHelper.GetIPTratado(), string.IsNullOrEmpty(erro));
            ViewBag.Mensagem = erro;
            return View(new string[] { pagina, htmlEditor });
        }

        public ActionResult Apagar(string pagina)
        {
            if (string.IsNullOrEmpty(pagina))
                return RedirectToAction(PaginaPadrao);
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(pagina);
            var conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo, IOHelper.DiasAExpirar.ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Apagar(string pagina, string conteudo = "")
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var apagados = IOHelper.ApagarLogicamentePaginaWithHistory(pagina);
            if (apagados > 0)
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Pagina - apagar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Exclusão de página: " + pagina + " - apagados: " + apagados, urlHelper.GetIPTratado(), apagados > 0);
                TempData["Mensagem"] = apagados + @" arquivo(s) apagado(s) com sucesso.";
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Pagina - apagar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Exclusão de página: " + pagina + " - apagados: " + apagados, urlHelper.GetIPTratado(), apagados > 0);
            ViewBag.Mensagem = "Nenhum arquivo apagado";
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(pagina);
            conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo });
        }

        public ActionResult Restaurar(string pagina)
        {
            if (string.IsNullOrEmpty(pagina))
                return RedirectToAction(PaginaPadrao);
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(pagina);
            var conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Restaurar(string pagina, string conteudo = "")
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var restaurados = IOHelper.RestaurarLogicamentePaginaWithHistory(pagina);
            if (restaurados > 0)
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Pagina - restaurar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Reuração de página: " + pagina + " - resturados: " + restaurados, urlHelper.GetIPTratado(), restaurados > 0);
                TempData["Mensagem"] = restaurados + " arquivo(s) restaurado(s) com sucesso.";
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Pagina - restaurar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Reuração de página: " + pagina + " - resturados: " + restaurados, urlHelper.GetIPTratado(), restaurados > 0);
            ViewBag.Mensagem = "Nenhum arquivo restaurado";
            var arquivoXML = IOHelper.GetPathFileXMLPadrao(pagina);
            conteudo = IOHelper.GetTextNodeXML(arquivoXML);
            return View(new string[] { pagina, conteudo });
        }
    }
}
