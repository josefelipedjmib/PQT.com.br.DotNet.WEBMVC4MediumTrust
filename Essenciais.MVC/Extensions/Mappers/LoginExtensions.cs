
using Domain.Entities;
using Essenciais.MVC.Models;

namespace Essenciais.MVC.Extensions.Mappers
{
    public static class LoginExtensions
    {
        public static Login MapToLogin(this UsuarioLogin usuarioLogin) => new Login()
        {
            Usuario = usuarioLogin.Usuario,
            Senha = usuarioLogin.Senha,
        };
    }
}