using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Xml;

namespace Auxiliar.Helper
{
    public static class IOHelper
    {
        public static string AppBaseDirectory = AppContext.BaseDirectory;
        public static int DiasAExpirar = 30;
        public static string XMLConeudoDiretorioPadrao = "Content/ctd/";
        public static string XMLExtensao = ".xml";
        public static string XMLModeloPadrao = "00modelo";
        public static string XMLNoPadraoConteudo = "//conteudo";
        public static string XMLDeletedArquivosMarca = "-DELETED-";
        public static string XMLConteudoHistoricoPadrao = @"([0-9]{4}.?[0-9]{2}.?[0-9]{2}.?[0-9]{2}.?[0-9]{2}.?[0-9]{2}).xml$";
        public static string XMLConteudoDeletadoPadrao = @".xml" + XMLDeletedArquivosMarca + @"[0-9]{14}$";

        public static string GetPathFileXMLPadrao(string arquivo)
        {
            if (arquivo.ToLower().Contains("_xml"))
            {
                arquivo = arquivo.Replace("_xml", XMLExtensao);
                return Path.Combine(AppBaseDirectory + XMLConeudoDiretorioPadrao + arquivo);
            }
            return Path.Combine(AppBaseDirectory + XMLConeudoDiretorioPadrao + arquivo + XMLExtensao);
        }

        public static string GetPathXMLPadrao()
        {
            return Path.Combine(AppBaseDirectory + XMLConeudoDiretorioPadrao);
        }

        public static string GetTextNodeXML(string arquivo, string nodeXML = "")
        {
            if (string.IsNullOrEmpty(nodeXML))
                nodeXML = XMLNoPadraoConteudo;
            var conteudo = GetNodeXML(arquivo, nodeXML);
            return conteudo?.InnerText ?? string.Empty;
        }

        public static XmlNode GetNodeXML(string arquivo, string nodeXML = "")
        {
            if (string.IsNullOrEmpty(nodeXML))
                nodeXML = XMLNoPadraoConteudo;
            try
            {
                return GetXMLFile(arquivo)?.SelectSingleNode(nodeXML);
            }
            catch (Exception ex) { };
            return null;
        }

        public static XmlDocument GetXMLFile(string arquivo)
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(arquivo);
                return xml;
            }
            catch (Exception ex) { };
            return null;
        }

        public static string[] GetAllFilesFromDir(string dir, string pattern = "*")
        {
            var arquivosXml = new string[] { };
            if (Directory.Exists(dir))
            {
                arquivosXml = Directory.EnumerateFiles(dir, pattern).ToArray();
            }
            return arquivosXml;
        }

        public static IEnumerable<string> GetAllXMLConteudoFiltered()
            => GetAllFilesFromDir(GetPathXMLPadrao(), "*" + XMLExtensao)
                                .Where(
                                    caminhoCompleto =>
                                        !Path.GetFileName(caminhoCompleto).StartsWith("00")
                                        && !Regex.IsMatch(Path.GetFileName(caminhoCompleto), XMLConteudoHistoricoPadrao)
                                );

        public static int GetTotalXMLConteudoFiltered()
            => GetAllXMLConteudoFiltered().Count();

        public static Retorno<List<Pagina>> GetAllXMLConteudosPadrao(int pagina = 1, int paginaTamanho = 0, List<OrdenacaoCampo> ordenacao = null)
        {
            var arquivosXml = GetAllXMLConteudoFiltered()
                                .Select(
                                    p =>
                                        new Pagina()
                                        {
                                            Arquivo = p,
                                            Nome = @Path.GetFileName(p).ToLower().Replace(".xml", ""),
                                            Data = File.GetLastWriteTime(p)
                                        }
                                ).ToList();
            if (ordenacao?.Any() ?? false)
            {
                var ordem = ordenacao.Find(o => o.Campo.ToLower().Equals("nome"));
                if(ordem != null)
                {
                    if (ordem.Direcao.ToLower().Equals("asc"))
                        arquivosXml = arquivosXml.OrderBy(a => a.Nome).ToList();
                    else
                        arquivosXml = arquivosXml.OrderByDescending(a => a.Nome).ToList();
                }
                ordem = ordenacao.Find(o => o.Campo.ToLower().Equals("arquivo"));
                if (ordem != null)
                {
                    if (ordem.Direcao.ToLower().Equals("asc"))
                        arquivosXml = arquivosXml.OrderBy(a => a.Arquivo).ToList();
                    else
                        arquivosXml = arquivosXml.OrderByDescending(a => a.Arquivo).ToList();
                }
                ordem = ordenacao.Find(o => o.Campo.ToLower().Equals("data"));
                if (ordem != null)
                {
                    if (ordem.Direcao.ToLower().Equals("asc"))
                        arquivosXml = arquivosXml.OrderBy(a => a.Data).ToList();
                    else
                        arquivosXml = arquivosXml.OrderByDescending(a => a.Data).ToList();
                }
            }
            if (paginaTamanho > 0)
            {
                arquivosXml = arquivosXml.Skip((pagina - 1) * paginaTamanho).Take(paginaTamanho).ToList();
            }
            return new Retorno<List<Pagina>>() { Data = arquivosXml };
        }

        public static string[] GetDeletedsXMLConteudosPadrao()
        {
            var conteudosDir = GetPathXMLPadrao();
            var arquivosXml = GetAllFilesFromDir(conteudosDir, "*")
                                .Where(
                                    caminhoCompleto =>
                                        Regex.IsMatch(Path.GetFileName(caminhoCompleto), XMLConteudoDeletadoPadrao)
                                        && !Regex.IsMatch(Path.GetFileName(caminhoCompleto), XMLConteudoHistoricoPadrao.Replace(".xml$", XMLConteudoDeletadoPadrao))
                                )
                                .ToArray();
            return arquivosXml;
        }

        public static string[] GetAllPaginaFilesWithHistory(string pagina)
        {
            var extensao = XMLExtensao.ToLower();
            var padrao = pagina + "( " + XMLConteudoHistoricoPadrao.Replace(".xml$", "") + ")?.xml$";
            if (pagina.ToLower().Contains("_xml"))
            {
                pagina = pagina.Replace("_xml", extensao);
                pagina = pagina.Substring(0, pagina.ToLower().IndexOf(extensao));
                extensao = "";
                padrao = pagina + "( " + XMLConteudoHistoricoPadrao.Replace(".xml$", "") + ")?.xml(" + XMLConteudoDeletadoPadrao.Replace(".xml", "").Replace("$", "") + ")$";
            }
            var paginasXml = GetAllFilesFromDir(GetPathXMLPadrao(), "*" + extensao)
                                .Where(
                                    caminhoCompleto =>
                                        Regex.IsMatch(Path.GetFileName(caminhoCompleto),
                                           padrao)
                                )
                                .ToArray();
            return paginasXml;
        }

        public static int ApagarLogicamentePaginaWithHistory(string pagina)
        {
            var apagados = 0;
            var arquivos = GetAllPaginaFilesWithHistory(pagina);
            if (arquivos?.Length > 0)
            {
                foreach (var arquivo in arquivos)
                {
                    File.Move(arquivo, arquivo + XMLDeletedArquivosMarca + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    apagados++;
                }
            }
            return apagados;
        }
        public static int RestaurarLogicamentePaginaWithHistory(string pagina)
        {
            var restaurados = 0;
            var arquivos = GetAllPaginaFilesWithHistory(pagina);
            if (arquivos?.Length > 0)
            {
                foreach (var arquivo in arquivos)
                {
                    var arquivoNovo = arquivo.Substring(0, arquivo.IndexOf(XMLExtensao) + XMLExtensao.Length);
                    if(!Regex.IsMatch(arquivoNovo, XMLConteudoHistoricoPadrao))
                    {
                        if (File.Exists(arquivoNovo))
                            File.Delete(arquivoNovo);
                    }
                    else
                    {
                        if (File.Exists(arquivoNovo))
                        {
                            var encontrado = Regex.Matches(arquivoNovo, XMLConteudoHistoricoPadrao.Replace(".xml$", ""));
                            if(encontrado.Count > 0)
                            {
                                var inicio = arquivoNovo.IndexOf(encontrado[0].Value);
                                var fim = inicio + encontrado[0].Value.Length;
                                var data = DateTime.Parse(encontrado[0].Value.Replace("_", ":"));
                                do
                                {
                                    data = data.AddSeconds(-1);
                                    arquivoNovo = arquivoNovo.Substring(0, inicio) + data.ToString("yyyy-MM-dd HH_mm_ss") + arquivoNovo.Substring(fim);
                                } while (File.Exists(arquivoNovo));
                            }
                        }
                    }
                    File.Move(arquivo, arquivoNovo);
                    restaurados++;
                }
            }
            return restaurados;
        }

        public static int ApagarXMLsDeletadosExpirados(int diasMaiorQue = 30)
        {
            if (diasMaiorQue < 1)
                diasMaiorQue = DiasAExpirar;
            var arquivosApagados = GetAllFilesFromDir(GetPathXMLPadrao())
                                .Where(
                                    caminhoCompleto =>
                                        Regex.IsMatch(Path.GetFileName(caminhoCompleto), XMLConteudoDeletadoPadrao)
                                )
                                .ToArray();
            var dataExpiracao = long.Parse(DateTime.Now.AddDays(-diasMaiorQue).ToString("yyyyMMddHHmmss"));
            var quantidadeApagados = 0;
            foreach (var arquivo in arquivosApagados)
            {
                var arquivoData = long.Parse(arquivo.Substring(arquivo.IndexOf(XMLDeletedArquivosMarca) + XMLDeletedArquivosMarca.Length));
                if (arquivoData < dataExpiracao)
                {
                    File.Delete(arquivo);
                    quantidadeApagados++;
                }
            }
            return quantidadeApagados;
        }

        public static string NovoXMLConteudoPadrao(string pagina, string htmlEditor)
        {
            var erro = "";
            try
            {
                if (!string.IsNullOrEmpty(pagina) && !string.IsNullOrEmpty(htmlEditor))
                {
                    var xml = GetXMLFile(GetPathFileXMLPadrao(pagina));
                    if (xml == null)
                    {
                        xml = GetXMLFile(GetPathFileXMLPadrao(XMLModeloPadrao));
                        var conteudo = xml.SelectSingleNode(XMLNoPadraoConteudo);
                        if (conteudo != null)
                        {
                            conteudo.InnerXml = "\n\t\t<![CDATA[\n" + htmlEditor + "\n\t\t]]>\n  ";
                            xml.Save(GetPathFileXMLPadrao(pagina));
                        }
                        else
                        {
                            erro = "Erro ao acessar conteúdo do arquivo";
                        }
                    }
                    else
                    {
                        erro = "A página já existe.";
                    }
                }
                else
                {
                    erro = "Página e Conteúdo não podem estar em branco.";
                }
            }
            catch (Exception ex) { erro = ex.ToString(); }
            return erro;
        }

        public static string EditXMLConteudoPadrao(string pagina, string htmlEditor)
        {
            var erro = "";
            if (!string.IsNullOrEmpty(pagina) && !string.IsNullOrEmpty(htmlEditor))
            {
                var xml = GetXMLFile(GetPathFileXMLPadrao(pagina));
                if (xml != null)
                {
                    xml.Save(GetPathFileXMLPadrao(pagina + DateTime.Now.ToString(" yyyy-MM-dd HH_mm_ss")));
                    var conteudo = xml.SelectSingleNode(XMLNoPadraoConteudo);
                    if (conteudo != null)
                    {
                        conteudo.InnerXml = "\n\t\t<![CDATA[\n" + htmlEditor + "\n\t\t]]>\n  ";
                        xml.Save(GetPathFileXMLPadrao(pagina));
                    }
                    else
                    {
                        erro = "Erro ao acessar conteúdo do arquivo";
                    }
                }
                else
                {
                    erro = "Erro ao acessar o arquivo.";
                }
            }
            else
            {
                erro = "Página e Conteúdo não podem estar em branco.";
            }
            return erro;
        }
    }
}
