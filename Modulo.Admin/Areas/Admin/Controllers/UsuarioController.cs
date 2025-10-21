using Essenciais.MVC.ActionFilters;
using Essenciais.MVC.Controllers;
using Essenciais.MVC.Extensions;
using Essenciais.MVC.Extensions.Mappers;
using Essenciais.MVC.Models;
using Domain.Models;
using Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auxiliar.Helper;
using System.Web.Routing;
using Essenciais.MVC;
using Service.Mail;
using System;

namespace Modulo.Admin.Areas.Admin.Controllers
{
    [AutorizadorActionFilter]
    public class UsuarioController : PaginatedController
    {
        private Dictionary<string, string> _configs = ConfigurationsManagerHelper.GetAllAppSettings(ConfigurationsManagerHelper.GetAppSettingsAsDictionary, null);
        private UsuarioService _usuarioService = new UsuarioService();
        private string _rootPath = "/admin/usuario/";

        // GET: Usuario/Action
        [PaginadaOrdenadaActionFilter]
        public ActionResult Index()
        {
            var mensagem = "";
            if (TempData["Mensagem"] != null)
                mensagem += TempData["Mensagem"].ToString();
            ViewBag.Mensagem = mensagem;
            if (!(Retorno.OrdenacaoAtual?.Any() ?? false))
                Retorno.OrdenacaoAtual = new List<OrdenacaoCampo>()
                {
                    new OrdenacaoCampo()
                    {
                        Campo = "nomeExibicao",
                        Direcao = "asc"
                    }
                };
            Retorno.Cabecalhos = new List<PropriedadeConfig>
                {
                    new PropriedadeConfig { NomePropriedade = "nomeExibicao", NomeExibicao = "" },
                    new PropriedadeConfig { NomePropriedade = "email", NomeExibicao = "E-mail" }
                };
            Retorno.TotalItens = _usuarioService.ContarRegistros().Data;
            if (Retorno.TotalItens < 1)
                Retorno.Errors.Add("Nenhum dado retornado.");
            else
            {
                var usuarios = _usuarioService.Listar("", "", 0, "", Retorno.PaginasNumero, Retorno.PaginasTamanho, Retorno.OrdenacaoAtual).Data;
                Retorno.Data = usuarios.Select(usuario =>
                    {
                        var dict = Retorno.TratarDict(usuario);
                        // Adiciona o Id ou chave para os links de ação
                        dict["Id"] = usuario.id;
                        return dict;
                    }).ToList();
            }
            Retorno.RootPath = _rootPath;
            Retorno.EhAdmin = true;
            return View(Retorno);
        }

        public ActionResult Detalhar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(PaginaPadrao);
            var usuario = (new Usuario()).MapFromUsuario(_usuarioService.Obter(id).Data);
            var retorno = new Retorno<Usuario>() { Data = usuario };
            retorno.RootPath = _rootPath;
            retorno.EhAdmin = true;
            return View(retorno);
        }

        public ActionResult Novo()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Novo(Usuario usuario)
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var alterado = _usuarioService.Salvar(usuario.MapToUsuario());
            if (!alterado.Errors.Any() && alterado.Data > 0)
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Usuario - novo", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Cadastro de usuário - e-mail: " + usuario.EMail + " - alterados: " + alterado.Data, urlHelper.GetIPTratado(), !alterado.Errors.Any());
                TempData["Mensagem"] = @"
                                Usuário criado com sucesso.
                            ";
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Usuario - novo", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Cadastro de usuário - e-mail: " + usuario.EMail + " - erros: " + alterado.Errors.Count + " - " + string.Join(" - ", alterado.Errors), urlHelper.GetIPTratado(), !alterado.Errors.Any());
            ViewBag.Mensagem = "Erro ao cadastrar o usuário. <br> Erros: <br> - " + string.Join(" <br> - ", alterado.Errors);
            return View(usuario);
        }

        public ActionResult Editar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(PaginaPadrao);
            var usuarioById = _usuarioService.Obter(id);
            if (usuarioById.Errors.Any())
            {
                ViewBag.Mensagem = "Erro ao obter o usuário solicitado. <br> Erros: <br> - " + string.Join(" <br> - ", usuarioById.Errors);
                usuarioById.Data = null;
            }
            var usuario = (new Usuario()).MapFromUsuario(usuarioById.Data);
            usuario.Senha = "";
            usuario.ConfirmaSenha = "";
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Editar(Usuario usuario)
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            if (
                !string.IsNullOrEmpty(usuario.Senha)
                && (!usuario.Senha.Equals(usuario.ConfirmaSenha))
            )
            {
                ViewBag.Mensagem = "Senha não confere com a confirmação.";
            }
            else
            {
                var usuarioAlterado = _usuarioService.Salvar(usuario.MapToUsuario(true));
                if (!usuarioAlterado.Errors.Any() && usuarioAlterado.Data > 0)
                {
                    var mensagem = usuarioAlterado.Mensagem;
                    if (
                        !string.IsNullOrEmpty(mensagem)
                        && mensagem.StartsWith("emailalterado"))
                    {
                        var mensagens = mensagem.Split(',');
                        if (mensagens.Length.Equals(3))
                        {
                            var emailNovo = usuario.EMail;
                            var emailAntigo = mensagens[1];
                            var activationKey = mensagens[2];
                            var variaveis = new List<KeyValuePair<string, string>>();
                            variaveis.Add(new KeyValuePair<string, string>("{{NOME_USUARIO}}", usuario.NomeExibicao));
                            variaveis.Add(new KeyValuePair<string, string>("{{SISTEMA_NOME}}", _configs["Sistema:Nome"]));
                            variaveis.Add(new KeyValuePair<string, string>("{{EMAIL_ANTIGO}}", emailAntigo));
                            variaveis.Add(new KeyValuePair<string, string>("{{EMAIL_NOVO}}", emailNovo));
                            variaveis.Add(new KeyValuePair<string, string>("{{URL_RESETA_EMAIL}}", urlHelper.GetBase() + URLsPadrao.ResetarEmail + "/" + activationKey));
                            variaveis.Add(new KeyValuePair<string, string>("{{PAGINA_REFERENCIA}}", Request.UrlReferrer.AbsoluteUri));
                            variaveis.Add(new KeyValuePair<string, string>("{{DATAHORA_EXTENSO}}", DateTime.Now.ToString("dddd, dd de MMMM de yyyy - HH:mm:ss")));
                            var enviado = await (new Mail())
                                .Prepare(_configs)
                                .Subject("Comunicado de alteração de e-mail")
                                .IsBodyHtml(true)
                                .BodyObterDoArquivo("reseta-email.html", variaveis)
                                .DefinirTelaSecao("")
                                .AdicionarDestinatarios(emailAntigo)
                                .Send();
                        }
                    }
                    Infrastructure.LogSystem.Prepare(_configs);
                    Infrastructure.LogSystem.Logar(logado, "Usuario - editar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Alteração de usuário - e-mail: " + usuario.EMail + " - alterados: " + usuarioAlterado.Data, urlHelper.GetIPTratado(), !usuarioAlterado.Errors.Any());
                    TempData["Mensagem"] = @"
                                Usuário " + usuario.NomeExibicao + @" alterado com sucesso.
                            ";
                    return RedirectToAction(PaginaPadrao);
                }
                else
                {
                    ViewBag.Mensagem = "Não foi possível alterar o usuário. <br> Erros: <br> - " + string.Join(" <br> - ", usuarioAlterado.Errors);
                }
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Usuario - editar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Alteração  de usuário - e-mail: " + usuario.EMail + " - erros: " + ViewBag.Mensagem, urlHelper.GetIPTratado(), false);
            usuario.Senha = "";
            usuario.ConfirmaSenha = "";
            return View(usuario);
        }

        public ActionResult Apagar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(PaginaPadrao);
            var usuario = (new Usuario()).MapFromUsuario(_usuarioService.Obter(id).Data);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Apagar(Usuario usuario)
        {
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
            var apagados = _usuarioService.Apagar(usuario.Id);
            if (!apagados.Errors.Any() && apagados.Data > 0)
            {
                Infrastructure.LogSystem.Prepare(_configs);
                Infrastructure.LogSystem.Logar(logado, "Usuario - apagar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Exclusão de usuário - e-mail: " + usuario.EMail + " - alterados: " + apagados.Data, urlHelper.GetIPTratado(), !apagados.Errors.Any());
                if (usuario.Id.Equals(logado))
                {
                    var urlSair = URLsPadrao.Sair.Split('/');
                    return RedirectToRoute(new RouteValueDictionary
                        (
                            new
                            {
                                controller = urlSair[1],
                                action = urlSair[2],
                                area = "Admin" //Rota padrão para deslogar
                            }
                        )
                    );
                }

                TempData["Mensagem"] = " Usuário " + usuario.NomeExibicao + " apagado com sucesso. Total de registros alterados: " + apagados.Data;
                return RedirectToAction(PaginaPadrao);
            }
            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Usuario - apagar", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Exclusão de usuário - e-mail: " + usuario.EMail + " - erros: " + apagados.Errors.Count + " - " + string.Join(" - ", apagados.Errors), urlHelper.GetIPTratado(), !apagados.Errors.Any());
            ViewBag.Mensagem = "Nenhum usuário apagado <br> Erros: <br> - " + string.Join(" <br> - ", apagados.Errors);
            return View(usuario);
        }
    }
}