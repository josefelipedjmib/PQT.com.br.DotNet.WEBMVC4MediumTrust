using System.Collections.Generic;
using Domain.Models;
using Domain.Interfaces;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace Essenciais.MVC.ActionFilters
{
    public class PaginadaOrdenadaAPIActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var controller = actionContext.ControllerContext;
            if (controller.Controller is IPaginado paginacaoController)
            {
                var requestUri = actionContext.Request.RequestUri;
                var queryString = System.Web.HttpUtility.ParseQueryString(requestUri.Query);

                if (int.TryParse(queryString["pagina"], out int paginaQuerystring) && paginaQuerystring > 0)
                {
                    paginacaoController.Retorno.PaginasNumero = paginaQuerystring;
                }

                if (int.TryParse(queryString["paginatamanho"], out int paginaTamanhoQueryString) && paginaTamanhoQueryString > 0)
                {
                    paginacaoController.Retorno.PaginasTamanho = paginaTamanhoQueryString;
                }

                // A lista de campos de ordenação. Ex: ["nome:asc", "data:desc"]
                paginacaoController.Retorno.OrdenacaoAtual = new List<OrdenacaoCampo>();
                var sortOrder = queryString["ordena"];
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    // Analisar a query string 'sortOrder'
                    foreach (var parte in sortOrder.Split(','))
                    {
                        var campos = parte.Split(':');
                        if (campos.Length == 2)
                        {
                            paginacaoController.Retorno.OrdenacaoAtual.Add(new OrdenacaoCampo { Campo = campos[0], Direcao = campos[1] });
                        }
                    }
                }
            }
        }
    }
}