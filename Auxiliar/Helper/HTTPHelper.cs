using System.Text.Json;
using RestSharp;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Domain.Interfaces;
using Domain.Models;
using System.Linq;

namespace Auxiliar.Helper
{
    public static class HTTPHelper
    {
        public static async Task<Y> BuscarDadosAPI<T, Y>(string TipoSolicitacao, string endpoint, string json = "", string tokenJwt = null) where Y : IRetorno<T>
        {
            try
            {
                var retorno = await BuscarDadosAPI(TipoSolicitacao, endpoint, json, tokenJwt);
                var mensagem = TratarMensagemRestResponse<T>(retorno, TipoSolicitacao);
                var objResponse = await CriarResponse<Y>(retorno, mensagem);

                if (objResponse == null)
                    return objResponse;

                if (!retorno.IsSuccessStatusCode)
                    objResponse.Errors.Add("Erro: " + retorno.ErrorMessage + " - " + retorno.ErrorException.Message);
                objResponse.Mensagem = $"{(int)retorno.StatusCode}-{retorno.StatusCode}:> ";
                if (!string.IsNullOrEmpty(objResponse?.Mensagem))
                    objResponse.Mensagem += objResponse.Mensagem;
                else
                    objResponse.Mensagem += mensagem;
                return objResponse;
            }
            catch (Exception e)
            {
                var retorno = new RestResponse() { Content = "", IsSuccessStatusCode = false, StatusCode = HttpStatusCode.InternalServerError, ErrorMessage = e.Message };
                var mensagem = e.Message;
                return await CriarResponse<Y>(retorno, mensagem);
            }
        }

        public static async Task<List<T>> BuscarListaAPI<T>(string endpoint)
        {
            try
            {
                var tipoSolicitacao = "GET";
                var retorno = await BuscarDadosAPI(tipoSolicitacao, endpoint);
                var messagem = TratarMensagemRestResponse<T>(retorno, tipoSolicitacao);
                var response = new HttpResponseMessage(retorno.StatusCode)
                {
                    Content = new StringContent(retorno.Content, Encoding.UTF8, "application/json"),
                    ReasonPhrase = messagem
                };
                var json = await response.Content.ReadAsStringAsync();
                var objResponse =
                    (response.IsSuccessStatusCode)
                        ? JsonSerializer.Deserialize<Retorno<List<T>>>(json)
                            : new Retorno<List<T>> { Errors = new List<string>() { "Erro: " + response.StatusCode }, Mensagem = response.ReasonPhrase };

                if (!objResponse.Errors.Any())
                {
                    return objResponse.Data;
                }
            }
            catch (Exception e)
            {
                var teste = e;
            }
            return new List<T>();
        }

        public static async Task<RestResponse> BuscarDadosAPI(string TipoSolicitacao, string endpoint, string json = "", string tokenJwt = null)
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest(endpoint);

                switch (TipoSolicitacao.ToUpper())
                {
                    case "GET":
                        request.Method = Method.Get;
                        break;
                    case "POST":
                        request.Method = Method.Post;
                        break;
                    case "PUT":
                        request.Method = Method.Put;
                        break;
                    case "DELETE":
                        request.Method = Method.Delete;
                        break;
                    default:
                        return new RestResponse();
                }

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                if (!string.IsNullOrEmpty(tokenJwt))
                    request.AddHeader("Authorization", "Bearer " + tokenJwt);
                request.AddHeader("Access-Control-Allow-Origin", "*");

                return await client.ExecuteAsync<RestResponse>(request);
            }
            catch (Exception e)
            {
                return new RestResponse() { Content = "", IsSuccessStatusCode = false, StatusCode = HttpStatusCode.InternalServerError, ErrorMessage = e.Message };
            }
        }

        private static async Task<T> CriarResponse<T>(RestResponse retorno, string mensagem)
        {
            var response = new HttpResponseMessage(retorno.StatusCode)
            {
                Content = new StringContent(retorno.Content, Encoding.UTF8, "application/json"),
                ReasonPhrase = mensagem
            };

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var objResponse = JsonSerializer.Deserialize<T>(jsonResponse);
            return objResponse;
        }

        private static string TratarMensagemRestResponse<T>(RestResponse retorno, string TipoSolicitacao)
        {
            var mensagem = "";
            try
            {
                if (retorno == null)
                    mensagem = "Erro ao consultar dados";
                else if (retorno.StatusCode != HttpStatusCode.OK && retorno.ErrorMessage != null)
                    mensagem = $"{retorno.ErrorMessage} - {retorno.ErrorException?.Message}";
                else
                {
                    var content = JsonSerializer.Deserialize<Retorno<T>>(retorno.Content);
                    if (content.Data != null && !content.Errors.Any())
                    {
                        var acaoRealizada = "";
                        switch (TipoSolicitacao.ToUpper())
                        {
                            case "GET":
                                acaoRealizada = "retornado";
                                break;
                            case "POST":
                                acaoRealizada = "salvo";
                                break;
                            case "PUT":
                                acaoRealizada = "alterado";
                                break;
                            case "DELETE":
                                acaoRealizada = "excluído";
                                break;
                            default:
                                acaoRealizada = "não realizado";
                                break;
                        }
                        mensagem = "Registro " + acaoRealizada + " com sucesso.";
                    }
                    else
                        mensagem = content?.Mensagem?.Replace(Environment.NewLine, "  ").Replace("\n", "  ") ?? "";
                }
            }
            catch (Exception e)
            {
                mensagem = "Erro ao tratar mensagem: " + e.Message;
            }
            return mensagem;
        }
    }
}
