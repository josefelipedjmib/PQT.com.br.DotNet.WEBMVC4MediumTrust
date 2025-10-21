using System.Collections.Generic;
using System.Web.Mvc;
using Domain.Models;
using Domain.Interfaces;

namespace Essenciais.MVC.ActionFilters
{
    public class PaginadaOrdenadaActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller;
            if (controller is IPaginado paginacaoController)
            {
                var request = filterContext.HttpContext.Request;
                if (int.TryParse(request.QueryString["pagina"], out int paginaQuerystring) && paginaQuerystring > 0)
                {
                    paginacaoController.Retorno.PaginasNumero = paginaQuerystring;
                }

                if (int.TryParse(request.QueryString["paginatamanho"], out int paginaTamanhoQueryString) && paginaTamanhoQueryString > 0)
                {
                    paginacaoController.Retorno.PaginasTamanho = paginaTamanhoQueryString;
                }

                // A lista de campos de ordenação. Ex: ["nome:asc", "data:desc"]
                paginacaoController.Retorno.OrdenacaoAtual = new List<OrdenacaoCampo>();
                var sortOrder = request.QueryString["ordena"];
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
            base.OnActionExecuting(filterContext);
        }
    }
}