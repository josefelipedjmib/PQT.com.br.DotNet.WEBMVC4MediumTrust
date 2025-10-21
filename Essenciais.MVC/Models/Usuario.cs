
using System;
using System.ComponentModel.DataAnnotations;

namespace Essenciais.MVC.Models
{
    public class Usuario
    {
        [MaxLength(30)]
        [Display(Name = "ID")]
        public string Id { get; set; }

        [MaxLength(50)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [MaxLength(50)]
        [Display(Name = "Sobre Nome")]
        public string SNome { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Nome de exibição")]
        public string NomeExibicao { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "E-mail")]
        public string EMail { get; set; }

        [MaxLength(255)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [MaxLength(255)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [MaxLength(255)]
        [Display(Name = "Redigite a Senha")]
        public string ConfirmaSenha { get; set; }

        [MaxLength(255)]
        [Display(Name = "Salt")]
        public string Salt { get; set; }

        [Display(Name = "Ativado")]
        public bool Activate { get; set; } = false;

        [MaxLength(255)]
        [Display(Name = "Chave de Ativação")]
        public string ActivationKey { get; set; }

        [Display(Name = "Atualizado em")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Criado em")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Exibir")]
        public bool Exibir { get; set; } = true;

        [Display(Name = "Excluido")]
        public bool Excluido { get; set; } = false;

        [Display(Name = "Aceito")]
        public bool Aceito { get; set; } = false;
    }
}