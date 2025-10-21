using Domain.Entities;
using Domain.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace RepositoryFramework
{
    public class UsuarioEmailHistoricoRepository : BaseRepository<UsuarioEmailHistorico, string>
    {
        public UsuarioEmailHistoricoRepository() : base() { }

        public Retorno<int> DeleteExpirados(int expirado = 30)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmd = new MySqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    var sql = new StringBuilder();
                    sql.AppendFormat("DELETE FROM {0} WHERE DATEDIFF(data, NOW()) < {1}", _table, expirado * -1);
                    cmd.CommandText = sql.ToString();
                    cn.Open();
                    retorno.Data = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
                throw ex;
            }
            return retorno;
        }

        public Retorno<int> DeleteEmailHistorico(string usuario, DateTime data)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmd = new MySqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    var sql = new StringBuilder();
                    sql.AppendFormat("DELETE FROM {0} WHERE usuario = @usuario and data >= @data", _table);
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@data", data);
                    cn.Open();
                    retorno.Data = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
                throw ex;
            }
            return retorno;
        }

        public UsuarioEmailHistorico GetByActivationKey(string activationKey)
            => List(activationKey, "activationKey", 1, "").Data?.FirstOrDefault();
    }
}
