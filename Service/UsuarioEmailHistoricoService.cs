using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using RepositoryFramework;
using System;

namespace Service
{
    public class UsuarioEmailHistoricoService : BaseService<UsuarioEmailHistorico, string>
    {
        protected override IRepository<UsuarioEmailHistorico, string> repo { get; set; } = new UsuarioEmailHistoricoRepository();

        public Retorno<int> DeleteEmailHistorico(string usuario, DateTime data)
            => (repo is UsuarioEmailHistoricoRepository usuarioEmailHistoricoRepo) ? usuarioEmailHistoricoRepo.DeleteEmailHistorico(usuario, data) : null;

        public Retorno<int> DeleteExpirados(int expirado = 30)
            => (repo is UsuarioEmailHistoricoRepository usuarioEmailHistoricoRepo) ? usuarioEmailHistoricoRepo.DeleteExpirados(expirado) : null;

        public UsuarioEmailHistorico GetByActivationKey(string activationKey)
        => (repo is UsuarioEmailHistoricoRepository usuarioEmailHistoricoRepo) ? usuarioEmailHistoricoRepo.GetByActivationKey(activationKey) : null;
    }
}
