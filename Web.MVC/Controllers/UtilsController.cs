using Essenciais.MVC.ActionFilters;
using System.Web.Http;
using Auxiliar.Extensions;
using Essenciais.MVC.Models;

namespace Web.MVC.Controllers
{
    [AutorizadorAPIActionFilter]
    public class UtilsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Criptografar([FromBody] string texto)
        {
            return Json(texto.Encrypt());
        }

        [HttpPost]
        public IHttpActionResult Descriptografar([FromBody] string texto)
        {
            return Json(texto.Decrypt());
        }

        [HttpPost]
        public IHttpActionResult GerarSenha([FromBody] UsuarioLogin usuario)
        {
            if (!string.IsNullOrEmpty(usuario.Usuario))
                usuario.Senha = (new PasswordEncryptor(usuario.Usuario)).EncryptPassword(usuario.Senha);
            else
            {
                var usuarioGerado = new Domain.Entities.Usuario();
                usuarioGerado.senha = usuario.Senha;
                usuario.Usuario = usuarioGerado.salt;
                usuario.Senha = usuarioGerado.senha;
                usuarioGerado = null;
            }
            return Json(usuario);
        }
    }
}