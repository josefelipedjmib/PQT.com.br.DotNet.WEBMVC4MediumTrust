using Domain.Interfaces;
using Domain.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Essenciais.MVC.Controllers
{
    public abstract class PaginatedController : Controller, IPaginado
    {
        protected string PaginaPadrao = "Index";
        public RetornoPaginado<List<Dictionary<string, object>>> Retorno { get; set; } = new RetornoPaginado<List<Dictionary<string, object>>>();

        public PaginatedController()
        {
            int paginaTamanho = 0;
            int.TryParse(ConfigurationManager.AppSettings["Paginacao:ItensPorPagina"], out paginaTamanho);
            if (paginaTamanho > 0)
                Retorno.PaginasTamanho = paginaTamanho;
            Retorno.PaginasNumero = 1;
        }
    }
}