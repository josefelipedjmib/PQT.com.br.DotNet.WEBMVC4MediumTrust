using System.Linq;
using Domain.Entities;

namespace Service
{
    public class AuthenticateService
    {
        public readonly static int ErrosPermitidos = 5;
        public static string SuperUser = "super";
        private UsuarioService _usuarioService = new UsuarioService();

        public static bool IsValid(string loginID)
        {
            return !string.IsNullOrEmpty(loginID)
                && loginID.Length > 2;
        }

        public static bool IsErrosExceeded(int errosOcorridos)
        {
            if (errosOcorridos < ErrosPermitidos)
                return false;
            return true;
        }

        public static Usuario GetUsuarioById(string id)
        {
            var usuario = new Usuario();
            if (id.Equals("000001"))
            {
                usuario.nomeExibicao = "Sistema Interno";
                usuario.email = "suporte@pqt.com.br";
            }
            else
                usuario = (new UsuarioService()).Listar(id, "id", 1).Data?.FirstOrDefault();
            return usuario;
        }

        public string Login(Login login)
        {
            if (
                login.Usuario.Equals(SuperUser)
                && (new PasswordEncryptor(PasswordEncryptor.SuperUserSalt)).EncryptPassword(login.Senha).Equals(PasswordEncryptor.SuperUserPasswordEncrypted))
                return "000001";
            else
            {
                var usuario = _usuarioService.GetByLogin(login.Usuario);
                if (usuario != null)
                {
                    var senha = (new PasswordEncryptor(usuario.salt)).EncryptPassword(login.Senha);
                    usuario = _usuarioService.GetByLoginAndPasswordAndActivate(login.Usuario, senha);
                    return usuario?.id ?? "";
                }
            }
            return "";
        }
    }
}
