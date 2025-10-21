using Domain.Interfaces;
using Domain.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

namespace Essenciais.MVC.Controllers
{
    public abstract class PaginatedApiController : ApiController, IPaginado
    {
        protected string PaginaPadrao = "Index";
        public RetornoPaginado<List<Dictionary<string, object>>> Retorno { get; set; } = new RetornoPaginado<List<Dictionary<string, object>>>();

        public PaginatedApiController()
        {
            int paginaTamanho = 0;
            int.TryParse(ConfigurationManager.AppSettings["Paginacao:ItensPorPagina"], out paginaTamanho);
            if (paginaTamanho > 0)
                Retorno.PaginasTamanho = paginaTamanho;
            Retorno.PaginasNumero = 1;
        }
    }
}