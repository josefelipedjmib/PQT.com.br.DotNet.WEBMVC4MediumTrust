using Domain.Attributes;
using System;

namespace Domain.Entities
{
    [Table("usuario")]
    public class Usuario
    {

        public Usuario()
        {
            senha = salt + PasswordEncryptor.GenerateRandomKey();
        }

        public string id { get; set; }
        public string nome { get; set; }
        public string snome { get; set; }
        public string nomeExibicao { get; set; }
        public string email { get; set; }
        [IgnoreField]
        public string salt { get; set; } = PasswordEncryptor.GenerateRandomKey();
        [IgnoreField]
        public string SenhaPreSalvar { get; private set; } = "";
        private string _senha;
        public void SetSenhaBruta(string senhaCriptografada)
        {
            _senha = senhaCriptografada;
        }
        [IgnoreField]
        public string senha
        {
            get { return _senha; }
            set
            {
                RegenerateActivationKey();
                SenhaPreSalvar = value;
                _senha = (new PasswordEncryptor(salt)).EncryptPassword(value);
            }
        }
        public bool activate { get; set; } = false;
        public string activationKey { get; set; }
        [IgnoreField]
        public DateTime? updatedAt { get; set; } = DateTime.Now;
        [IgnoreField]
        public DateTime? createdAt { get; set; } = DateTime.Now;
        public bool exibir { get; set; } = true;
        public bool excluido { get; set; } = false;

        public void RegenerateActivationKey()
        {
            activationKey = PasswordEncryptor.GenerateActivationKey(email);
        }
    }
}