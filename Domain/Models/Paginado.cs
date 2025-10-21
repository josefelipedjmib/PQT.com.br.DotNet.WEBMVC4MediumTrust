using System.Collections.Generic;

namespace Domain.Models
{
    public class Paginado
    {
        public int TotalItens { get; set; } = 0;
        public int PaginasNumero { get; set; } = 1;
        public int PaginasTamanho { get; set; } = 0;
        public int TotalPaginas { get; set; } = 1;
        public List<OrdenacaoCampo> OrdenacaoAtual { get; set; } = new List<OrdenacaoCampo>();
        public string RootPath { get; set; }
    }
}
