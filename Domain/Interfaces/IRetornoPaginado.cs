using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IRetornoPaginado<T> : IRetorno<T>
    {
        int TotalItens { get; set; }
        int PaginasNumero { get; set; }
        int PaginasTamanho { get; set; }
        int TotalPaginas { get; set; }
        List<PropriedadeConfig> Cabecalhos { get; set; }
        List<OrdenacaoCampo> OrdenacaoAtual { get; set; }
    }
}
