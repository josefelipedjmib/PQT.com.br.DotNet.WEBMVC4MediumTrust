
using Service;
using System.Web;
using System.Web.SessionState;

namespace Essenciais.MVC.Extensions
{
    public static class SessionExtensions
    {
        private static string _login = "login";
        private static string _contadorErro = "contadorErro";

        public static void SetLogin(this HttpSessionStateBase session, string value)
        {
            session.ResetaErro();
            session[_login] = value;
        }

        public static string GetLogin(this HttpSessionStateBase session)
        {
            return session[_login]?.ToString();
        }

        public static string GetLogin(this HttpSessionState session)
        {
            return session[_login]?.ToString();
        }

        public static bool IsLoginValid(this HttpSessionStateBase session)
        {
            return AuthenticateService.IsValid(GetLogin(session));
        }

        public static bool IsLoginValid(this HttpSessionState session)
        {
            return AuthenticateService.IsValid(GetLogin(session));
        }

        public static void SetErro(this HttpSessionStateBase session, int value)
        {
            session[_contadorErro] = value;
        }

        private static void SetErro(this HttpSessionState session, int value)
        {
            session[_contadorErro] = value;
        }

        public static int GetErro(this HttpSessionState session)
        {
            var erro = 0;
            int.TryParse(session[_contadorErro]?.ToString(), out erro);
            return erro;
        }

        public static int GetErro(this HttpSessionStateBase session)
        {
            var erro = 0;
            int.TryParse(session[_contadorErro]?.ToString(), out erro);
            return erro;
        }

        public static void ResetaErro(this HttpSessionStateBase session)
        {
            SetErro(session, 0);
        }

        public static void IncrementaErro(this HttpSessionStateBase session)
        {
            var erros = GetErro(session);
            erros++;
            SetErro(session, erros);
        }

        public static void IncrementaErro(this HttpSessionState session)
        {
            var erros = GetErro(session);
            erros++;
            SetErro(session, erros);
        }
    }
}