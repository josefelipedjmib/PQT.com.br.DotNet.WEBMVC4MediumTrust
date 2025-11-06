using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Sys = System.Configuration;
using System.Data;
using Auxiliar.Extensions;
using Auxiliar.Helper;
using System.Linq;
using System.Text;
using Domain.Attributes;
using Domain.Models;
using Domain.Interfaces;

namespace RepositoryFramework
{
    public abstract class BaseRepository<T, TPrimaryKey> : IRepository<T, TPrimaryKey> where T : class, new()
    {
        protected string _table = "";
        protected string _sqlCommandStart = "sql>";
        private Type _tipo = typeof(T);
        protected string conexao = Sys.ConfigurationManager.ConnectionStrings["conexaoMySQL"].ConnectionString.Decrypt() + "Allow Zero Datetime=True;Convert Zero Datetime=True;";
        public virtual string ListWhere { get; protected set; } = "WHERE {{field}} LIKE @{{field}}";
        public virtual string WhereAtTheEnd { get; protected set; } = "  ";

        public BaseRepository()
        {
            var tabelaAttr = (TableAttribute)Attribute.GetCustomAttribute(_tipo, typeof(TableAttribute));
            if (string.IsNullOrEmpty(_table))
                _table = tabelaAttr?.Name ?? _tipo.Name;
        }

        public Retorno<int> Save(T entidade, string idPropertyName = "id", Dictionary<string, string> customInsert = null, Dictionary<string, string> customUpdate = null)
        {
            if (customInsert == null)
                customInsert = new Dictionary<string, string>();
            if (customUpdate == null)
                customUpdate = new Dictionary<string, string>();

            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                var propriedades = _tipo.GetProperties();
                var idProp = propriedades.FirstOrDefault(p => p.Name.Equals(idPropertyName, StringComparison.OrdinalIgnoreCase));
                var idValue = idProp != null ? idProp.GetValue(entidade, null) : null;

                var isInsert = idValue == null || int.Equals(idValue, 0) || string.IsNullOrEmpty(idValue.ToString());
                var sql = new StringBuilder();
                var parametros = new List<MySqlParameter>();


                // Filtra propriedades que não têm o atributo IgnoreField
                var propriedadesValidas = propriedades
                    .Where(p => !Attribute.IsDefined(p, typeof(IgnoreFieldAttribute)))
                    .ToList();

                if (isInsert)
                {
                    sql.AppendFormat("INSERT INTO `{0}` (", _table);
                    var campos = new List<string>();
                    var valores = new List<string>();

                    foreach (var prop in propriedadesValidas)
                    {
                        if (prop.Name.Equals(idPropertyName, StringComparison.OrdinalIgnoreCase)) continue;

                        campos.Add($"`{prop.Name}`");
                        valores.Add($"@{prop.Name}");
                        var valor = prop.GetValue(entidade, null);
                        parametros.Add(new MySqlParameter($"@{prop.Name}", valor ?? DBNull.Value));
                    }

                    foreach (var custom in customInsert)
                    {
                        campos.Add($"`{custom.Key}`");
                        if (custom.Value.IndexOf(_sqlCommandStart) == 0)
                        {
                            valores.Add(custom.Value.Replace(_sqlCommandStart, ""));
                        }
                        else
                        {
                            valores.Add($"@{custom.Key}");
                            parametros.Add(new MySqlParameter($"@{custom.Key}", custom.Value));
                        }
                    }

                    sql.Append(string.Join(", ", campos));
                    sql.Append(") VALUES (");
                    sql.Append(string.Join(", ", valores));
                    sql.Append(")");
                }
                else
                {
                    sql.AppendFormat("UPDATE `{0}` SET ", _table);
                    var sets = new List<string>();

                    foreach (var prop in propriedadesValidas)
                    {
                        if (prop.Name.Equals(idPropertyName, StringComparison.OrdinalIgnoreCase)) continue;

                        sets.Add($"`{prop.Name}` = @{prop.Name}");
                        var valor = prop.GetValue(entidade, null);
                        parametros.Add(new MySqlParameter($"@{prop.Name}", valor ?? DBNull.Value));
                    }

                    foreach (var custom in customUpdate)
                    {
                        if (custom.Value.IndexOf(_sqlCommandStart) == 0)
                        {
                            sets.Add($"`{custom.Key}` = {custom.Value.Replace(_sqlCommandStart, "")}");
                        }
                        else
                        {
                            sets.Add($"`{custom.Key}` = @{custom.Key}");
                            parametros.Add(new MySqlParameter($"@{custom.Key}", custom.Value));
                        }
                    }
                    sql.Append(string.Join(", ", sets));
                    sql.AppendFormat(" WHERE `{0}` = @{0}  " + WhereAtTheEnd, idPropertyName);
                    parametros.Add(new MySqlParameter($"@{idPropertyName}", idValue));
                }

                using (var cn = new MySqlConnection(conexao))
                using (var cmd = new MySqlCommand(sql.ToString(), cn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(parametros.ToArray());
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

        public Retorno<T> Get(TPrimaryKey codigo, string idPropertyName = "id")
        {
            var retorno = new Retorno<T>() { Data = new T() };
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmd = new MySqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $@"
                            SELECT 
                                *
                            FROM {_table} 
                                WHERE {idPropertyName} = @id {WhereAtTheEnd}
                        ";

                    cmd.Parameters.AddWithValue(idPropertyName, codigo);

                    MySqlDataReader dr;
                    cn.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        retorno.Data = preencherDadosRetorno<T>(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
                throw ex;
            }
            return retorno;
        }

        public Retorno<int> CountRows(string value = "", string byField = "", int returnRows = 0, string where = "")
        {
            var retorno = new Retorno<int>() { Data = 0 };
            where = WhereTreated(where);
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmdCount = CMDPatternQuery(byField, value, where);
                    cmdCount.Connection = cn;
                    cmdCount.CommandText = cmdCount.CommandText.Replace("SELECT * FROM", "SELECT COUNT(*) FROM");
                    cn.Open();
                    retorno.Data = Convert.ToInt32(cmdCount.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
                throw ex;
            }
            return retorno;
        }

        public Retorno<List<T>> List(string value = "", string byField = "", int returnRows = 0, string where = "", int pagina = 1, int paginaTamanho = 0, List<OrdenacaoCampo> ordenacao = null)
        {
            var retorno = new Retorno<List<T>>();
            where = WhereTreated(where);
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmd = CMDPatternQuery(byField, value, where);
                    cmd.Connection = cn;
                    var sql = new StringBuilder();
                    sql.Append(cmd.CommandText);

                    if (ordenacao?.Any() ?? false)
                    {
                        sql.AppendLine("  ORDER BY ");
                        var orderBy = ordenacao.Select(x => $"{x.Campo} {x.Direcao.ToUpper()}").ToArray();
                        sql.AppendLine(string.Join(", ", orderBy) + "  ");
                    }

                    if (returnRows > 0)
                    {
                        sql.AppendLine("  LIMIT " + returnRows + "  ");
                    }
                    else if (paginaTamanho > 0)
                    {
                        pagina = pagina < 1 ? 1 : pagina;
                        int offset = (pagina - 1) * paginaTamanho;
                        sql.AppendLine("  LIMIT " + paginaTamanho + " OFFSET " + offset + "  ");
                    }

                    MySqlDataReader dr = null;
                    cmd.CommandText = sql.ToString();
                    cn.Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var lista = preencherDadosRetornoList<T>(dr).ToList();
                    retorno.Data = lista;
                }
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
                throw ex;
            }
            return retorno;
        }

        public Retorno<int> Delete(TPrimaryKey codigo, bool deleteLogic = false, string idPropertyName = "id")
        {
            var retorno = new Retorno<int>() {  Data = 0 };
            try
            {
                using (var cn = new MySqlConnection(conexao))
                {
                    var cmd = new MySqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    var sql = new StringBuilder();
                    if (deleteLogic)
                        sql.AppendFormat("UPDATE {0} SET excluido = 1 WHERE {1} = @id {2}", _table, idPropertyName, WhereAtTheEnd);
                    else
                        sql.AppendFormat("DELETE FROM {0} WHERE {1} = @id {2}", _table, idPropertyName, WhereAtTheEnd);

                    cmd.Parameters.AddWithValue(idPropertyName, codigo);
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

        public bool Existe(string value, string byField)
        {
            return List(value, byField, 1)?.Data?.Any() ?? false;
        }

        protected string WhereTreated(string where)
        {
            if (string.IsNullOrEmpty(where))
                where = ListWhere;
            return "  " + where + "  ";
        }

        protected MySqlCommand CMDPatternQuery(string byField, string value, string where)
        {
            var cmd = new MySqlCommand();
            cmd.CommandType = CommandType.Text;
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM {0}", _table);

            if (!string.IsNullOrEmpty(byField))
            {
                sql.AppendLine(where.Replace("{{field}}", byField));
                cmd.Parameters.AddWithValue($"@{byField}", value);
            }
            else
            {
                sql.AppendLine("  WHERE TRUE  ");
            }
            sql.AppendLine(WhereAtTheEnd);
            cmd.CommandText += sql.ToString();
            return cmd;
        }

        protected virtual List<T> preencherDadosRetornoList<T>(MySqlDataReader dr) where T : new()
        {
            var obj = new T();
            var lista = new List<T>();
            while (dr.Read())
            {
                lista.Add(preencherDadosRetorno<T>(dr));
            }
            return lista;
        }

        protected virtual T preencherDadosRetorno<T>(MySqlDataReader dr) where T : new()
        {
            T obj = new T();
            obj = MySQLHelper.Mapear<T>(dr);
            return obj;
        }
    }
}
