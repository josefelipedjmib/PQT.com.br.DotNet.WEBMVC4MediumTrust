using Domain.Attributes;
using System;

namespace Domain.Entities
{
    [Table("usuario_emailhistorico")]
    public class UsuarioEmailHistorico
    {
        public string usuario { get; set; }
        [IgnoreField]
        public DateTime data { get; set; } = DateTime.Now;
        public string activationKey { get; set; }
        public string email { get; set; }
        public void RegenerateActivationKey()
        {
            activationKey = PasswordEncryptor.GenerateActivationKey(email);
        }
    }
}