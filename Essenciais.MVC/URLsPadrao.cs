using System.Collections.Generic;

namespace Essenciais.MVC
{
    public static class URLsPadrao
    {
        public static string ActionDefault => "Index";
        public static string Login => "/Admin";
        // Fazer com que as URLs, sigam padrão /Controller/Action
        public static string Painel => "/Admin/Painel";
        public static string Cadastro => "/Admin/Cadastro";
        public static string Ativar => "/Admin/Ativar";
        public static string Resetar => "/Admin/Resetar";
        public static string ResetarEmail => "/Admin/ResetarEmail";
        public static string Sair => "/Admin/Sair";

        public static Dictionary<string, string> MenuAdministrativo()
        {
            return new Dictionary<string, string>()
            {
                { "Usuários", "/Admin/Usuario/" },
                { "Páginas", "/Admin/Pagina/" },
                { "Informações do Servidor", "/Admin/Informacoes/" },
            };
        }
    }
}