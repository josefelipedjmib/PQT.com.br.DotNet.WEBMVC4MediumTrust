using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IPaginado
    {
        RetornoPaginado<List<Dictionary<string, object>>> Retorno { get; set; }
    }
}