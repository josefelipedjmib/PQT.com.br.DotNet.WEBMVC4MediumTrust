using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IRetorno<T>
    {
        List<string> Errors { get; set; }
        string Mensagem { get; set; }
        string RootPath { get; set; }
        bool EhAdmin { get; set; }
        T Data { get; set; }
    }
}
