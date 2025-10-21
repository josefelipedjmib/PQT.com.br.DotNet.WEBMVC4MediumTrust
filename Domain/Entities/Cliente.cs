using Domain.Attributes;

namespace Domain.Entities
{
    [Table("CLIENTE")]
    public class Cliente
    {
        public int id { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public string endereco { get; set; }
    }
}
