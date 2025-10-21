using System.ComponentModel.DataAnnotations;

namespace Essenciais.MVC.Models
{
    public class UsuarioLogin
    {
        [Required]
        [MaxLength(250)]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }
}