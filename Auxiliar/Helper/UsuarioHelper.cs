using Auxiliar.Enums;

namespace Auxiliar.Helper
{
    public static class UsuarioHelper
    {
        readonly static int PAPEL_INVALIDO = 0;
        private static int _role = 0;
        readonly static int USUARIO_INVALIDO = 0;
        private static int _usuario = 0;

        public static bool EhAdminOuMembro(string role)
        {
            return EhAdmin(role) || EhMembro(role);
        }

        public static bool EhAdmin(string role)
        {
            if (role != null)
            {
                int tipoLogin = PAPEL_INVALIDO;
                int.TryParse(role, out tipoLogin);
                if (tipoLogin.Equals((int)TipoLoginEnum.Admin))
                    return true;
            }
            return false;
        }

        public static bool EhMembro(string role)
        {
            if (role != null)
            {
                int tipoLogin = PAPEL_INVALIDO;
                int.TryParse(role, out tipoLogin);
                if (tipoLogin.Equals((int)TipoLoginEnum.Membro))
                    return true;
            }
            return false;
        }

        public static int PegaRole(string role)
        {
            _role = PAPEL_INVALIDO;
            if (!string.IsNullOrEmpty(role))
            {
                int.TryParse(role, out _role); ;
            }
            return _role;
        }

        public static int PegaUsuarioID(string usuario)
        {
            _usuario = USUARIO_INVALIDO;
            if (!string.IsNullOrEmpty(usuario))
            {
                int.TryParse(usuario, out _usuario);
            }
            return _usuario;
        }

    }
}
