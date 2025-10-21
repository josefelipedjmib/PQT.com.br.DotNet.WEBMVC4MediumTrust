
using Domain.Entities;

namespace Essenciais.MVC.Extensions.Mappers
{
    public static class UsuarioExtensions
    {
        public static Usuario MapToUsuario(this Models.Usuario usuarioModel, bool forcaSenhaSalt = false)
        {
            if (usuarioModel == null)
                return null;
            var usuario = new Usuario()
            {
                id = usuarioModel.Id,
                nome = usuarioModel.Nome,
                snome = usuarioModel.SNome,
                nomeExibicao = usuarioModel.NomeExibicao,
                email = usuarioModel.EMail,
                activate = usuarioModel.Activate,
                exibir = usuarioModel.Exibir,
            };
            usuario.senha = forcaSenhaSalt ? usuarioModel.Senha : usuarioModel.Senha ?? usuario.senha;
            usuario.salt = forcaSenhaSalt ? usuario.salt : usuarioModel.Salt ?? usuario.salt;
            usuario.activationKey = usuarioModel.ActivationKey ?? usuario.activationKey;
            usuario.updatedAt = usuarioModel.UpdatedAt ?? usuario.updatedAt;
            usuario.createdAt = usuarioModel.CreatedAt ?? usuario.createdAt;
            return usuario;
        }

        public static Models.Usuario MapFromUsuario(this Models.Usuario usuarioModel, Usuario usuario)
        {
            if (usuario == null)
                return null;
            return new Models.Usuario()
            {
                Id = usuario.id,
                Aceito = false,
                Activate = usuario.activate,
                ActivationKey = usuario.activationKey,
                CreatedAt = usuario.createdAt,
                EMail = usuario.email,
                Exibir = usuario.exibir,
                Nome = usuario.nome,
                NomeExibicao = usuario.nomeExibicao,
                Salt = usuario.salt,
                Senha = usuario.senha,
                SNome = usuario.snome,
                UpdatedAt = usuario.updatedAt
            };
        }
    }
}