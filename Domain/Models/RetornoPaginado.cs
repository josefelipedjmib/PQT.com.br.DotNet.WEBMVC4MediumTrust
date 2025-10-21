using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Models
{
    public class RetornoPaginado<T> : Retorno<T>
    {
        private int _totalItens = 0;
        public int TotalItens
        {
            get { return _totalItens; }
            set
            {
                _totalItens = value;
                CalcularTotalPaginas();
            }
        }
        public int PaginasNumero { get; set; } = 1;
        public int PaginasTamanho { get; set; } = 0;
        public int TotalPaginas { get; private set; } = 1;
        public List<PropriedadeConfig> Cabecalhos { get; set; }
        public List<OrdenacaoCampo> OrdenacaoAtual { get; set; } = new List<OrdenacaoCampo>();

        public Paginado GetPaginado() => new Paginado()
        {
            PaginasNumero = PaginasNumero,
            PaginasTamanho = PaginasTamanho,
            TotalItens = TotalItens,
            TotalPaginas = TotalPaginas,
            OrdenacaoAtual = OrdenacaoAtual,
            RootPath = RootPath
        };

        public void CalcularTotalPaginas()
        {
            if (!Errors.Any())
                TotalPaginas = (int)Math.Ceiling((double)TotalItens / PaginasTamanho);
            else
                TotalPaginas = 0;
            if (PaginasNumero > TotalPaginas) PaginasNumero = TotalPaginas;
            if(PaginasNumero < 1) PaginasNumero = 1;
        }

        public Dictionary<string, object> TratarDict(object obj)
        {
            var dict = new Dictionary<string, object>();
            foreach (var propConfig in Cabecalhos)
            {
                var propInfo = obj.GetType().GetProperty(propConfig.NomePropriedade);
                dict[propConfig.NomeExibicao] = propInfo?.GetValue(obj);
            }
            return dict;
        }
    }
}
