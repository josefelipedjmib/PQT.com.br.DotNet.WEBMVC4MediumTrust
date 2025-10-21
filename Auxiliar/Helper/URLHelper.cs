using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;

namespace Auxiliar.Helper
{
    public class URLHelper
    {
        private readonly string _versao = "2.0";
        private readonly string _ipLocal = "127.0.0.1";
        private string _dir = "";
        private string _subpasta = "modelo";

        private Uri _uri;
        private string _scheme;
        private string _host;
        private string _baseUrl;
        private int _port;
        public string Pagina = "principal";
        public string IPAddress { get; private set; }


        public URLHelper(string fullUrl, string ipAddress, string paginaPadrao = "principal")
        {
            if (Pagina != paginaPadrao)
                Pagina = paginaPadrao;
            var testaURL = fullUrl.Split(';');
            if(testaURL.Length > 1 && testaURL[1].ToLower().StartsWith("http"))
                fullUrl = testaURL[1];
            _uri = new Uri(fullUrl);
            _scheme = _uri.Scheme;
            _host = _uri.Host;
            _port = _uri.Port;
            _baseUrl = $"{_scheme}://{_host + ((_host.ToLower().Contains("localhost") && _port > 0) ? ":" + _port : "")}";

            string paginaURL = GetTratada(_uri.PathAndQuery).Replace(_subpasta + "/", "");
            string paginaDir = "";

            if (paginaURL == _subpasta)
            {
                paginaURL = "";
                paginaDir = _subpasta + "/";
            }

            var arrPaginaURL = paginaURL.Split('/');
            for (int i = 1; i < arrPaginaURL.Length; i++)
            {
                paginaDir += "../";
            }

            paginaURL = RemoveUltimaBarra(paginaURL);

            if (!string.IsNullOrEmpty(paginaURL))
            {
                Pagina = paginaURL;
            }

            _dir = paginaDir;
            IPAddress = ipAddress;
        }

        public string GetUri()
        {
            return _uri.ToString();
        }

        public string GetPaginaURL(string uri)
        {
            var paginaUrl = GetTratada(uri);
            paginaUrl = RemoveUltimaBarra(paginaUrl);
            if (!string.IsNullOrEmpty(paginaUrl))
            {
                Pagina = paginaUrl;
            }
            return Pagina;
        }

        public string GetPaginaArquivo(string uri)
        {
            return GetPaginaURL(uri).Replace("/", "-");
        }

        public string GetTratada(string uri)
        {
            var url = Regex.Matches(uri, @"\/?([^?]+).*", RegexOptions.IgnoreCase);
            if (url.Count.Equals(0) || url[0].Value.Equals("/"))
                return "";
            return url[0].Groups[1].Value;
        }

        public string GetUltimaParte(string url)
        {
            var parts = url.Split('/');
            return string.IsNullOrEmpty(parts[parts.Length - 1]) ? parts[parts.Length - 2] : parts[parts.Length - 1];
        }

        public bool ValidaURLExterna(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && !string.IsNullOrEmpty(uriResult.Host);
        }

        public string GetRootPath(string path)
        {
            string root = "/";
            if (path.StartsWith(_subpasta))
            {
                root += _subpasta + "/";
            }
            return root;
        }

        public string GetBase()
        {
            return _baseUrl;
        }

        public string GetHost()
        {
            return _host;
        }

        public string GetScheme()
        {
            return _scheme;
        }

        public string GetPagina()
        {
            return Pagina.Replace("/", "-");
        }

        public string GetDir()
        {
            return _dir;
        }

        public string GetHostName()
        {
            return Dns.GetHostName();
        }

        public string GetIP()
        {
            return IPAddress;
        }

        public string[] GetIPTratado()
        {
            var ipPartes = (IPAddress+"").Split('.');
            if (ipPartes.Length != 4)
            {
                var resolvedIP = Dns.GetHostAddresses(GetHostName())[0].ToString();
                ipPartes = resolvedIP.Split('.');
                if (ipPartes.Length != 4)
                    ipPartes = _ipLocal.Split('.');
            }
            return ipPartes;
        }

        public string[] GetSegmentos()
        {
            return _uri.Segments;
        }

        public string[] GetSegmentosTratado()
        {
            var segmentos = GetSegmentos();
            var ultimo = GetSegmentos().Last();
            ultimo = RemoveUltimaBarra(ultimo);
            segmentos[segmentos.Length - 1] = ultimo;
            return segmentos;
        }

        public string GetURLAteSegmento(int indice)
        {
            var url = "/";
            for (var i = 1; i <= indice; i++)
            {
                url += RemoveUltimaBarra(GetSegmentos()[i]) + "/";
            }
            return url;
        }

        public string RemoveUltimaBarra(string texto)
        {
            return texto.TrimEnd('/');
        }

        public string GetVersao()
        {
            return _versao;
        }
    }
}
