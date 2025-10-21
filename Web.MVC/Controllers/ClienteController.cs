using Domain.Models;
using Essenciais.MVC.ActionFilters;
using Essenciais.MVC.Controllers;
using Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Web.MVC.Controllers
{
    public class ClienteController : PaginatedApiController
    {
        private ClienteService _clienteService = new ClienteService();

        [PaginadaOrdenadaAPIActionFilter]
        public IHttpActionResult Get()
        {
            Retorno.TotalItens = _clienteService.ContarRegistros().Data;
            if (Retorno.TotalItens < 1)
                Retorno.Errors.Add("Nenhum dado retornado.");
            else
            {
                Retorno.Cabecalhos = new List<PropriedadeConfig>
                {
                    new PropriedadeConfig { NomePropriedade = "id", NomeExibicao = "Id" },
                    new PropriedadeConfig { NomePropriedade = "cpf", NomeExibicao = "CPF" },
                    new PropriedadeConfig { NomePropriedade = "nome", NomeExibicao = "Nome" },
                    new PropriedadeConfig { NomePropriedade = "telefone", NomeExibicao = "Telefone" },
                    new PropriedadeConfig { NomePropriedade = "endereco", NomeExibicao = "Endereço" }
                };
                var clientes = _clienteService.Listar("", "", 0, "", Retorno.PaginasNumero, Retorno.PaginasTamanho, Retorno.OrdenacaoAtual).Data;
                Retorno.Data = clientes.Select(cliente =>
                {
                    var dict = Retorno.TratarDict(cliente);
                    // Adiciona o Id ou chave para os links de ação
                    dict["Id"] = cliente.id;
                    return dict;
                }).ToList();
            }
            return Json(Retorno);
        }

        // Referência da Versão antiga da API

        // GET api/<controller>
        //public IEnumerable<Cliente> Get()
        //{
        //    var lista = _clienteService.Listar();
        //    return lista;
        //}

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}