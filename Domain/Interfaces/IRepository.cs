using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IRepository<T, TPrimaryKey> where T : class, new()
    {
        Retorno<int> Save(T entidade, string idPropertyName = "id", Dictionary<string, string> customInsert = null, Dictionary<string, string> customUpdate = null);

        Retorno<T> Get(TPrimaryKey codigo, string idPropertyName = "id");
        Retorno<int> CountRows(string value = "", string byField = "", int returnRows = 0, string where = "");
        Retorno<List<T>> List(string value = "", string byField = "", int returnRows = 0, string where = "", int pagina = 1, int paginaTamanho = 0, List<OrdenacaoCampo> ordenacao = null);
        Retorno<int> Delete(TPrimaryKey codigo, bool deleteLogic = false, string idPropertyName = "id");
        bool Existe(string value, string byField);
    }
}
