using Domain.Entities;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Auxiliar.Helper;
using System.Linq;
using Domain.Models;

namespace RepositoryFramework
{
    public class UsuarioRepository : BaseRepository<Usuario, string>
    {
        public override string WhereAtTheEnd { get; protected set; } = " AND excluido = 0 AND ID <> '000001'  ";

        public UsuarioRepository() : base() { }

        public Retorno<int> Save(Usuario usuario)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            // usuário interno não pode ser alterado
            if (usuario.id == "000001")
            {
                retorno.Errors.Add("Usuário não encontrado.");
            } // validar senha se segue o padrão mínimo
            else if (!string.IsNullOrEmpty(usuario.senha) && !AutenticacaoHelper.ValidaSenha(usuario.SenhaPreSalvar))
            {
                retorno.Errors.Add("Senha não validada. (Precisa ter mais de 6 caracteres, maiúscula, minúscula e número)");
            } 
            else
            {
                var usuarioEmail = GetByEmail(usuario.email);
                // verificar se já possuí usuário associado ao e-mail informado
                if (
                    (
                        string.IsNullOrEmpty(usuario.id)
                        && usuarioEmail != null
                    )
                    || (
                        usuarioEmail != null
                        && usuarioEmail.id != usuario.id
                    )
                )
                {
                    retorno.Errors.Add("E-mail já associado a um usuário.");
                }
                else
                {
                    var customInsert = new Dictionary<string, string> { { "id", _sqlCommandStart + "usuarioID()" }, { "createdAt", _sqlCommandStart + "NOW()" }, { "updatedAt", _sqlCommandStart + "NOW()" } };
                    var customUpdate = new Dictionary<string, string> { { "updatedAt", _sqlCommandStart + "NOW()" } };
                    if (string.IsNullOrEmpty(usuario.id))
                    {
                        if (!string.IsNullOrEmpty(usuario.senha))
                        {
                            customInsert.Add("senha", usuario.senha);
                            customInsert.Add("salt", usuario.salt);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(usuario.senha))
                        {
                            customUpdate.Add("senha", usuario.senha);
                            customUpdate.Add("salt", usuario.salt);
                        }
                    }
                    retorno = base.Save(usuario, "id", customInsert, customUpdate);
                }
            }
            return retorno;
        }

        public Retorno<Usuario> Get(string codigo)
        {
            var retorno = new Retorno<Usuario>() { Data = null };
            if (codigo == "000001")
                retorno.Errors.Add("Usuário não encontrado");
            else
                retorno = base.Get(codigo);
            return retorno;
        }

        public Retorno<int> Delete(string codigo, bool deleteLogic = true)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            if (codigo == "000001")
                retorno.Errors.Add("Usuário não encontrado.");
            else
                retorno = base.Delete(codigo, deleteLogic);
            return retorno;
        }

        public Usuario GetByEmail(string email)
            => List(email, "email", 1, "").Data?.FirstOrDefault();

        public Usuario GetByEmailAndActivationKey(string email, string activationKey)
            => List("", "ignora", 1, "WHERE email = '" + email + "' and activationKey = '" + activationKey + "' ").Data?.FirstOrDefault();

        public Usuario GetByLogin(string login)
            => GetByEmail(login);

        public Usuario GetByLoginAndPasswordAndActivate(string usuario, string senha)
            => List("", "senha", 1, " WHERE email = '" + usuario + "' and senha = '" + senha + "' and activate = 1 ").Data?.FirstOrDefault();

        protected override T preencherDadosRetorno<T>(MySqlDataReader dr)
        {
            T obj = new T();
            obj = MySQLHelper.Mapear<T>(dr);

            if (obj is Usuario usuario)
            {
                usuario.SetSenhaBruta(TextoHelper.ByVarBinaryMySQL(dr["senha"]));
                usuario.createdAt = MySQLHelper.TratarDateTime(dr["createdAt"]);
                usuario.updatedAt = MySQLHelper.TratarDateTime(dr["updatedAt"]);
            }
            return obj;
        }
    }
}
