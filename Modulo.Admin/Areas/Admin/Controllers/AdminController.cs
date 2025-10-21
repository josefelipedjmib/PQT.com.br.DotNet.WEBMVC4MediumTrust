using Service;
using System.Web.Mvc;
using Essenciais.MVC;
using Essenciais.MVC.ActionFilters;
using Essenciais.MVC.Extensions;
using Essenciais.MVC.Extensions.Mappers;
using Models = Essenciais.MVC.Models;
using Auxiliar.Helper;
using Service.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Domain.Entities;

namespace Modulo.Admin.Areas.Admin.Controllers
{
    [AutorizadorActionFilter]
    public class AdminController : Controller
    {
        private AuthenticateService _authService = new AuthenticateService();
        private UsuarioService _usuarioService = new UsuarioService();
        private Dictionary<string, string> _configs = ConfigurationsManagerHelper.GetAllAppSettings(ConfigurationsManagerHelper.GetAppSettingsAsDictionary, null);

        // GET: Admin/Action
        [RedirecionarPadraoSeLogadoFilter]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (TempData["Mensagem"] != null)
                ViewBag.Mensagem = TempData["Mensagem"].ToString();
            return View(new Models.UsuarioLogin());
        }

        [HttpPost]
        [RedirecionarPadraoSeLogadoFilter]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.UsuarioLogin loginModel)
        {
            if (!AuthenticateService.IsErrosExceeded(Session.GetErro()))
            {
                if (ModelState.IsValid)
                {
                    var loginID = _authService.Login(loginModel.MapToLogin());
                    bool logged = AuthenticateService.IsValid(loginID);
                    if (logged)
                    {
                        var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
                        Response.Clear();
                        Session.SetLogin(loginID);
                        Infrastructure.LogSystem.Prepare(_configs);
                        Infrastructure.LogSystem.Logar(loginID, "Usuario - logado com sucesso", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Usuário logado - ID: " + loginID, urlHelper.GetIPTratado(), logged);
                        return Redirect(URLsPadrao.Painel);
                    }
                }
            }
            ViewBag.Erro = true;
            Session.IncrementaErro();
            return View(loginModel);
        }

        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        public ActionResult Cadastro()
        {
            return View(new Models.Usuario());
        }

        [HttpPost]
        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cadastro(Models.Usuario usuarioModel)
        {
            if (usuarioModel.Aceito)
            {
                if (ModelState.IsValid)
                {
                    var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
                    var usuario = usuarioModel.MapToUsuario();
                    var usuarioSalvo = _usuarioService.Salvar(usuario);
                    var cadastrado = !usuarioSalvo.Errors.Any() && usuarioSalvo.Data > 0;
                    var erros = "";
                    if (usuarioSalvo.Errors.Any())
                        erros = string.Join(" \n ", usuarioSalvo.Errors);
                    if (cadastrado)
                    {
                        TempData["Mensagem"] = @"
                                Usuário cadastrado com sucesso.<br> 
                                Verifique o seu e-mail, para o qual, enviamos uma mensagem para liberar o seu acesso.<br> 
                                Caso não apareça a mensagem, verifique se está no SPAM ou lixo eletrônico.
                            ";
                        var variaveis = new List<KeyValuePair<string, string>>();
                        variaveis.Add(new KeyValuePair<string, string>("{{NOME_USUARIO}}", usuario.nomeExibicao));
                        variaveis.Add(new KeyValuePair<string, string>("{{SISTEMA_NOME}}", _configs["Sistema:Nome"]));
                        variaveis.Add(new KeyValuePair<string, string>("{{URL_ATIVACAO}}", urlHelper.GetBase() + URLsPadrao.Resetar + "/" + usuario.activationKey));
                        variaveis.Add(new KeyValuePair<string, string>("{{PAGINA_REFERENCIA}}", Request.UrlReferrer.AbsoluteUri));
                        variaveis.Add(new KeyValuePair<string, string>("{{DATAHORA_EXTENSO}}", DateTime.Now.ToString("dddd, dd de MMMM de yyyy - HH:mm:ss")));
                        var enviado = await (new Mail())
                            .Prepare(_configs)
                            .Subject("Confirmação de Cadastro")
                            .IsBodyHtml(true)
                            .BodyObterDoArquivo("add-user.html", variaveis)
                            .DefinirTelaSecao("")
                            .AdicionarDestinatarios(usuario.email)
                            .Send();
                    }
                    else
                    {
                        TempData["Mensagem"] = @"
                               Houve um problema na finalização do cadastro. Por favor, tente novamente após alguns minutos.<br> Erros: <br>
                            " + erros.Replace("\n", "<br>");
                    }
                    var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
                    Infrastructure.LogSystem.Prepare(_configs);
                    Infrastructure.LogSystem.Logar(logado, "Usuario - adicionado", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Cadastro de usuário - e-mail: " + usuario.email + " - erros?: " + erros, urlHelper.GetIPTratado(), cadastrado);
                    return Redirect(URLsPadrao.Login);
                }
            }
            ViewBag.Erro = true;
            return View(usuarioModel);
        }

        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        public ActionResult Ativar(string id)
        {
            if (!_usuarioService.Existe(id, "activationKey"))
            {
                TempData["Mensagem"] = @"
                               Link expirado.
                            ";
                return Redirect(URLsPadrao.Login);
            }
            var usuario = new Models.Usuario() { ActivationKey = id };
            return View(usuario);
        }

        [HttpPost]
        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Ativar(Models.Usuario usuarioModel)
        {
            if (usuarioModel.Aceito)
            {
                if (
                    usuarioModel.NovaSenha.Equals(usuarioModel.ConfirmaSenha))
                {
                    var usuario = _usuarioService.GetByEmailAndActivationKey(usuarioModel.EMail, usuarioModel.ActivationKey);
                    if (usuario != null)
                    {
                        var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
                        usuario.senha = usuarioModel.NovaSenha;
                        usuario.activate = true;
                        var usuarioSalvo = _usuarioService.Salvar(usuario);
                        var cadastrado = !usuarioSalvo.Errors.Any() && usuarioSalvo.Data > 0;
                        var erros = "";
                        if (usuarioSalvo.Errors.Any())
                            erros = string.Join(" \n ", usuarioSalvo.Errors);
                        var logado = (Session.IsLoginValid()) ? Session.GetLogin() : "";
                        Infrastructure.LogSystem.Prepare(_configs);
                        Infrastructure.LogSystem.Logar(logado, "Usuario - ativação", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "ativação de usuário - e-mail: " + usuario.email + " - erros?: " + erros, urlHelper.GetIPTratado(), cadastrado);
                        if (cadastrado)
                            TempData["Mensagem"] = @"
                                    Usuário com acesso liberado.<br> 
                                    Senha alterada com sucesso.
                                ";
                        else
                            TempData["Mensagem"] = @"
                                    Usuário não liberado.<br> 
                                    Erros: <br> " + erros.Replace("\n", "<br>");
                        return Redirect(URLsPadrao.Login);
                    }
                    else
                    {
                        ViewBag.Mensagem = "Informações de usuário incorretas.";
                    }
                }
                else
                {
                    ViewBag.Mensagem = "A senha e sua confirmação não conferem.";
                }
            }
            ViewBag.Erro = true;
            return View(usuarioModel);
        }

        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        public ActionResult Resetar()
        {
            return View(new Models.Usuario());
        }

        [HttpPost]
        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resetar(Models.Usuario usuarioModel)
        {
            if (usuarioModel.Aceito)
            {
                var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
                var usuario = _usuarioService.GetByEmail(usuarioModel.EMail);
                return await ResetarUsuario(usuario, urlHelper);
            }
            ViewBag.Erro = true;
            return View(usuarioModel);
        }

        [HttpPost]
        public async Task<ActionResult> ResetarUsuario()
        {
            string id = Request.Form["id"];
            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
            var usuario = _usuarioService.Obter(id).Data;
            return await ResetarUsuario(usuario, urlHelper);
        }

        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        public ActionResult ResetarEmail(string id)
        {
            var emailHistoricoService = new UsuarioEmailHistoricoService();
            emailHistoricoService.DeleteExpirados();
            if (!emailHistoricoService.Existe(id, "activationKey"))
            {
                TempData["Mensagem"] = @"
                               Link expirado.
                            ";
                return Redirect(URLsPadrao.Login);
            }
            var usuario = new Models.Usuario() { ActivationKey = id };
            return View(usuario);
        }

        [HttpPost]
        [RedirecionarPadraoSeLogadoFilter]
        [AtivadoConfigActionFilter]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetarEmail(Models.Usuario usuarioModel)
        {
            var emailHistoricoService = new UsuarioEmailHistoricoService();
            if (usuarioModel.Aceito)
            {
                if (
                    usuarioModel.NovaSenha.Equals(usuarioModel.ConfirmaSenha))
                {
                    emailHistoricoService.DeleteExpirados();
                    var emailHistorico = emailHistoricoService.GetByActivationKey(usuarioModel.ActivationKey);
                    if (emailHistorico != null && emailHistorico.email.ToLower().Equals(usuarioModel.EMail.ToLower()))
                    {
                        var erros = "";
                        bool cadastrado = false;
                        var usuario = _usuarioService.Obter(emailHistorico.usuario).Data;
                        if (usuario != null)
                        {
                            var urlHelper = new URLHelper(Request.Url.ToString(), Request.UserHostAddress);
                            usuario.senha = usuarioModel.NovaSenha;
                            usuario.email = emailHistorico.email;
                            usuario.activate = true;
                            var usuarioSalvo = _usuarioService.Salvar(usuario);
                            cadastrado = !usuarioSalvo.Errors.Any() && usuarioSalvo.Data > 0;
                            if (usuarioSalvo.Errors.Any())
                                erros = string.Join(" \n ", usuarioSalvo.Errors);
                            Infrastructure.LogSystem.Prepare(_configs);
                            Infrastructure.LogSystem.Logar("", "Usuario - resetaEmail", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "reset de usuário - e-mail: " + usuario.email + " - erros?: " + erros, urlHelper.GetIPTratado(), cadastrado);
                        }
                        if (cadastrado)
                        {
                            emailHistoricoService.DeleteEmailHistorico(emailHistorico.usuario, emailHistorico.data);
                            TempData["Mensagem"] = @"
                                    Usuário com acesso liberado.<br> 
                                    Senha alterada com sucesso.
                                ";
                        }
                        else
                            TempData["Mensagem"] = @"
                                    Usuário não liberado.<br> 
                                    Erros: <br> " + erros.Replace("\n", "<br>");
                        return Redirect(URLsPadrao.Login);
                    }
                    else
                    {
                        ViewBag.Mensagem = "Informações de usuário incorretas.";
                    }
                }
                else
                {
                    ViewBag.Mensagem = "A senha e sua confirmação não conferem.";
                }
            }
            ViewBag.Erro = true;
            return View(usuarioModel);
        }

        [AllowAnonymous]
        public ActionResult Sair()
        {
            Session.Clear();
            return RedirectToAction(URLsPadrao.ActionDefault);
        }

        public ActionResult Painel()
        {
            var usuario = AuthenticateService.GetUsuarioById(Session.GetLogin());
            var usuarioModel = (new Models.Usuario()).MapFromUsuario(usuario);
            return View(usuarioModel);
        }

        public ActionResult Informacoes()
        {
            return View();
        }

        private async Task<ActionResult> ResetarUsuario(Usuario usuario, URLHelper urlHelper)
        {
            if (usuario != null)
            {
                var variaveis = new List<KeyValuePair<string, string>>();
                variaveis.Add(new KeyValuePair<string, string>("{{NOME_USUARIO}}", usuario.nomeExibicao));
                variaveis.Add(new KeyValuePair<string, string>("{{SISTEMA_NOME}}", _configs["Sistema:Nome"]));
                variaveis.Add(new KeyValuePair<string, string>("{{URL_ATIVACAO}}", urlHelper.GetBase() + URLsPadrao.Ativar + "/" + usuario.activationKey));
                variaveis.Add(new KeyValuePair<string, string>("{{PAGINA_REFERENCIA}}", Request.UrlReferrer.AbsoluteUri));
                variaveis.Add(new KeyValuePair<string, string>("{{DATAHORA_EXTENSO}}", DateTime.Now.ToString("dddd, dd de MMMM de yyyy - HH:mm:ss")));
                var enviado = await (new Mail())
                    .Prepare(_configs)
                    .Subject("Redefinição de senha")
                    .IsBodyHtml(true)
                    .BodyObterDoArquivo("reset-password.html", variaveis)
                    .DefinirTelaSecao("")
                    .AdicionarDestinatarios(usuario.email)
                    .Send();
            }

            TempData["Mensagem"] = @"
                                Enviamos uma mensagem para você com link de redefinição de senha.<br>
                                Caso não apareça a mensagem, verifique se está no SPAM ou lixo eletrônico do e-mail.
                            ";
            var logado = Session.IsLoginValid() ? Session.GetLogin() : usuario != null ? usuario.id : "";

            Infrastructure.LogSystem.Prepare(_configs);
            Infrastructure.LogSystem.Logar(logado, "Usuario - reset senha", $"{RouteData.Values["controller"]}/{RouteData.Values["action"]}", "Pedido de reset de senha com o e-mail: " + usuario.email, urlHelper.GetIPTratado(), usuario != null);
            return Redirect(URLsPadrao.Login);
        }
    }
}