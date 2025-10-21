using Auxiliar.Helper;
using RepositoryFramework;
using System;
using System.Collections.Generic;
using System.IO;
namespace Infrastructure
{
    public static class LogSystem
    {
        private static LogSystemRepository _repo = new LogSystemRepository();
        private static bool _usedPrepare = false;
        private static bool _useBD = false;
        private static bool _useArquivo = false;
        private static string _dataFormato = "dddd, dd - MMMM - yyyy - HH:mm:ss";
        private static string _dirPadrao = "LogErro\\";
        private static Dictionary<string, string> _config;

        public static void Prepare(Dictionary<string, string> config)
        {
            try
            {
                _config = config;
                bool.TryParse(_config["Logger:BD"], out _useBD);
                bool.TryParse(_config["Logger:Arquivo"], out _useArquivo);
                _dataFormato = string.IsNullOrEmpty(_config["Logger:DataFormato"].ToString()) ? _dataFormato : _config["Logger:DataFormato"].ToString();
                _dirPadrao = string.IsNullOrEmpty(_config["Logger:Dir"].ToString()) ? _dirPadrao : _config["Logger:Dir"].ToString();
                _usedPrepare = true;
            }
            catch (Exception ex) { }
        }

        public static void Logar(string usuario, string acao, string path, string status, string[] ip, bool sucesso, bool bd = true, bool arquivo = true)
        {
            if (!_usedPrepare) throw new Exception("método Prepare não utilizado corretamente.");
            if (_useBD && bd)
                LogarBD(usuario, acao + ">" + path, path + ">" + status, ip, sucesso);
            _usedPrepare = true;
            if (_useArquivo && arquivo)
                LogarArquivo(usuario, acao, path, status, ip, sucesso);
            _usedPrepare = false;
        }

        public static void LogarBD(string usuario, string acao, string status, string[] ip, bool sucesso)
        {
            if (!_usedPrepare) throw new Exception("método Prepare não utilizado corretamente.");
            if (string.IsNullOrEmpty(usuario))
                usuario = "000001";

            var logSystem = new Domain.Entities.LogSystem()
            {
                usuario = usuario,
                acao = acao,
                status = status,
                datahora = DateTime.Now,
                ipa = NumeroHelper.NumeroTextoEmInt(ip[0]),
                ipb = NumeroHelper.NumeroTextoEmInt(ip[1]),
                ipc = NumeroHelper.NumeroTextoEmInt(ip[2]),
                ipd = NumeroHelper.NumeroTextoEmInt(ip[3]),
                id = 0,
                sucesso = sucesso
            };
            _repo.Save(logSystem);
            _usedPrepare = false;
        }

        public static void LogarArquivo(string usuario, string acao, string path, string status, string[] ip, bool sucesso)
        {
            if (!_usedPrepare) throw new Exception("método Prepare não utilizado corretamente.");
            if (string.IsNullOrEmpty(usuario))
                usuario = "usuário não informado.";
            var sucessoOuErro = sucesso ? "sucesso" : "erro";
            var mensagem = "----------------------------------\n";
            mensagem += "----------------------------------\n";
            mensagem += DateTime.Now.ToString(_dataFormato);
            mensagem += "\n----------------------------------\n";
            mensagem += sucessoOuErro.ToUpper() + " na ação " + acao + " - path: " + path + "\n";
            mensagem += "Usuário: " + usuario + " - ip: " + string.Join(".", ip) + "\n";
            mensagem += "Detalhes do " + sucessoOuErro + " \n" + status + " \n";
            mensagem += "\n----------------------------------\n\n\n";

            var caminhoRaiz = AppContext.BaseDirectory;
            var arquivo = Path.Combine(_dirPadrao, acao + "--" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            if (arquivo.IndexOf("~/") != 0 && !Directory.Exists(_dirPadrao))
                throw new Exception("CaminhoRaiz inexistente: " + _dirPadrao);
            arquivo = arquivo.Replace("~/", caminhoRaiz);
            File.AppendAllText(arquivo, mensagem);
            _usedPrepare = false;
        }

        public static void Listar()
        {
            var teste = _repo.List();
        }
    }
}
