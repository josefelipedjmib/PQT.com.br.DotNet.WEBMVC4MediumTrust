using System.Collections.Generic;

namespace Domain.Models
{
    public class Retorno<T>
    {
        public List<string> Errors { get; set; } = new List<string>();
        public string Mensagem { get; set; }
        public string RootPath { get; set; }
        public bool EhAdmin { get; set; } = false;
        public T Data { get; set; }
    }
}