using System;
using System.Linq;

namespace Auxiliar.Extensions
{
    public static class UriExtensions
    {

        public static string[] GetSegmentsTrated(this Uri uri)
        {
            if (uri == null)
                return new string[] { };
            var segmentos = uri.Segments;
            var ultimo = uri.Segments.Last();
            ultimo = ultimo.RemoveFinalSlash();
            segmentos[segmentos.Length - 1] = ultimo;
            return segmentos;
        }

        public static string GetURLAteSegmento(this Uri uri, int indice)
        {
            if (uri == null)
                return "/";
            var url = "/";
            for (var i = 1; i <= indice; i++)
            {
                url += uri.Segments[i].RemoveFinalSlash() + "/";
            }
            return url;
        }

        public static string GetBaseURL(this Uri uri)
        {
            if (uri == null)
                return "/";
            return $"{uri.Scheme}://{uri.Host + ((uri.Host.ToLower().Contains("localhost") && uri.Port > 0) ? ":" + uri.Port : "")}";
        }
    }
}
