using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using RepositoryFramework;
using Service.Mail;
using System.Linq;

namespace Service
{
    public class UsuarioService : BaseService<Usuario, string>
    {
        protected override IRepository<Usuario, string> repo { get; set; } = new UsuarioRepository();

        public Retorno<int> Salvar(Usuario usuario)
        {
            var emailAlterado = "";
            var emailAntigo = "";
            if (!string.IsNullOrEmpty(usuario.id))
            {
                var usuarioDB = Obter(usuario.id);
                if (usuarioDB != null)
                {
                    emailAntigo = usuarioDB.Data.email.ToLower();
                    if (!emailAntigo.Equals(usuario.email.ToLower()))
                        emailAlterado = usuario.email.ToLower();
                }
            }
            var retorno = (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.Save(usuario) : null;
            if (!retorno.Errors.Any() && !string.IsNullOrEmpty(emailAlterado))
            {
                var emailHistorico = new UsuarioEmailHistorico()
                {
                    usuario = usuario.id,
                    email = emailAntigo
                };
                emailHistorico.RegenerateActivationKey();
                var logEmail = (new UsuarioEmailHistoricoService()).Salvar(emailHistorico);
                if (logEmail.Data > 0)
                    retorno.Mensagem = "emailalterado," + emailAntigo + "," + emailHistorico.activationKey;
            }
            return retorno;
        }

        public Retorno<Usuario> Obter(string codigo)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.Get(codigo) : null;

        public Retorno<int> Apagar(string codigo)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.Delete(codigo) : null;

        public Usuario GetByEmail(string email)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.GetByEmail(email) : null;

        public Usuario GetByEmailAndActivationKey(string email, string activationKey)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.GetByEmailAndActivationKey(email, activationKey) : null;

        public Usuario GetByLogin(string login)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.GetByLogin(login) : null;

        public Usuario GetByLoginAndPasswordAndActivate(string usuario, string senha)
            => (repo is UsuarioRepository usuarioRepo) ? usuarioRepo.GetByLoginAndPasswordAndActivate(usuario, senha) : null;
    }
}
