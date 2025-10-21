using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Auxiliar.Helper
{
    public static class TextoHelper
    {

        public static List<string> QuebrarEmPartes(string texto, int tamanho = 20)
        {
            return ChunkText(texto, tamanho);
        }

        public static string TirarCaracteresEspeciais(string texto)
        {
            var acentuada = "ŠŒŽšœžŸ¥µÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýÿ";
            var semacento = "SOZsozYYuAAAAAAACEEEEIIIIDNOOOOOOUUUUYsaaaaaaaceeeeiiiionoooooouuuuyy";

            for (var i = 0; i < acentuada.Length; i++)
                texto = texto.Replace(acentuada[i], semacento[i]);
            return Regex.Replace(texto, "[^a-zA-Z0-9-_.]+", "-");
        }

        public static List<string> ChunkText(string texto, int tamanho)
        {
            var chunks = new List<string>();
            for (int i = 0; i < texto.Length; i += tamanho)
            {
                int chunkSize = Math.Min(tamanho, texto.Length - i);
                chunks.Add(new string(texto.Skip(i).Take(chunkSize).ToArray()));
            }
            return chunks;
        }

        public static string ByVarBinaryMySQL(object value) => value.GetType().Name.Equals("DBNull") ? "" : Encoding.UTF8.GetString((byte[]) value);
        public static string ByVarCharMySQL(object value) => (value ?? "").ToString();

    }
}
