using System;
using System.Collections.Generic;
using System.Data;

namespace Auxiliar.Helper
{
    public static class MySQLHelper
    {
        private static readonly Dictionary<Type, Func<object, object>> _tratadores = new Dictionary<Type, Func<object, object>>
    {
        { typeof(string), v => TextoHelper.ByVarCharMySQL((string)v) },
        { typeof(DBNull), v => null },
        { typeof(DateTime), v => TratarDateTime(v) },
        { typeof(bool), v => ((bool)v).Equals(true) },
        { typeof(int), v => (int)v },
        { typeof(float), v => (float)v },
        { typeof(decimal), v => Convert.ToDecimal(v) },
        { typeof(double), v => Convert.ToDouble(v) },
        { typeof(byte[]), v => TextoHelper.ByVarBinaryMySQL((byte[])v) }
    };

        public static object Tratar(object valor)
        {
            if (valor == null || valor is DBNull) return null;

            var tipo = valor.GetType();
            if (_tratadores.TryGetValue(tipo, out var tratador))
                return tratador(valor);

            return valor;
        }

        public static T Mapear<T>(IDataReader reader) where T : new()
        {
            var obj = new T();
            var tipo = typeof(T);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var nomeColuna = reader.GetName(i);
                var valorBruto = reader.GetValue(i);
                var valorTratado = Tratar(valorBruto);

                var propriedade = tipo.GetProperty(nomeColuna);
                if (propriedade != null && propriedade.CanWrite)
                {
                    try
                    {
                        var valorConvertido = Convert.ChangeType(valorTratado, propriedade.PropertyType);
                        propriedade.SetValue(obj, valorConvertido, null);
                    }
                    catch
                    {
                        // Ignora conversões inválidas silenciosamente
                    }
                }
            }

            return obj;
        }

        public static DateTime? TratarDateTime(object valor)
        {
            if (valor == null || valor is DBNull)
                return null;

            DateTime dt;
            try
            {
                dt = Convert.ToDateTime(valor);
            }
            catch
            {
                return null;
            }
            if (dt == DateTime.MinValue || dt.Year < 1753 || dt.Year > 9999)
                return null;
            return dt;

            if (valor is string s && DateTime.TryParse(s, out DateTime resultado))
                return resultado;

            return null;
        }
    }
}
