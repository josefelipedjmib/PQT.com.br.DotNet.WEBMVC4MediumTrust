using System.Globalization;
using System.Text.RegularExpressions;

namespace Auxiliar.Helper
{
    public static class NumeroHelper
    {
        public static string RetornarApenasNumeros(string texto)
        {
            return Regex.Replace(texto, @"[^\d]", "");
        }

        public static string ReverterNumeroFormatado(string valor, string lang = "pt-BR")
        {
            if (string.IsNullOrEmpty(valor))
                return "0";
            var separadorMilhar = Regex.Replace(1111.ToString("N0", new CultureInfo(lang)), @"\s|\d", "").Trim();
            var separadorDecimal = Regex.Replace(1.11.ToString("N", new CultureInfo(lang)), @"\s|\d", "").Trim();
            var simboloMoeda = Regex.Replace(1.ToString("C", new CultureInfo(lang)), @"\s|\d|[" + separadorDecimal + "" + separadorMilhar + "]*", "").Trim();
            return valor.Replace(separadorMilhar, "").Replace(separadorDecimal, ".").Replace(simboloMoeda, "").Trim();
        }

        public static string NumeroFormatado(string valor, string lang = "pt-BR")
        {
            if (string.IsNullOrEmpty(valor))
                valor = "0";
            decimal valorConverter = decimal.Parse(valor, new CultureInfo(lang));
            return valorConverter.ToString("N2", new CultureInfo("pt-BR"));
        }

        public static string NumeroFormatadoBrasil(string valor)
        {
            return NumeroFormatado(ReverterNumeroFormatado(valor), "en-us");
        }

        public static decimal NumeroTextoEmDecimal(string valor)
        {
            decimal retorno = 0;
            decimal.TryParse(NumeroFormatadoBrasil(valor).Replace(".", "").Replace(",", ""), out retorno);
            if (retorno > 0)
                retorno /= 100;
            return retorno;
        }

        public static int NumeroTextoEmInt(string valor)
        {
            int retorno = 0;
            int.TryParse(valor, out retorno);
            return retorno;
        }
    }
}
