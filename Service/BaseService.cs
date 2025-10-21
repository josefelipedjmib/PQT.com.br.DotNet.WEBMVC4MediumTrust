using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Service
{
    public abstract class BaseService<T, TPrimaryKey> where T : class, new()
    {
        protected abstract IRepository<T, TPrimaryKey> repo { get; set; }

        //CRUD
        public Retorno<int> Salvar(T obj)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                retorno = repo.Save(obj);
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public Retorno<T> Obter(TPrimaryKey codigo)
        {
            var retorno = new Retorno<T>();
            try
            {
                retorno = repo.Get(codigo);
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public Retorno<int> ContarRegistros(string value = "", string byField = "", int returnRows = 0, string where = "")
        {
            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                retorno = repo.CountRows(value, byField, returnRows, where);
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public Retorno<List<T>> Listar(string value = "", string byField = "", int returnRows = 0, string where = "", int pagina = 1, int paginaTamanho = 0, List<OrdenacaoCampo> ordenacao = null)
        {
            var retorno = new Retorno<List<T>>();
            try
            {
                retorno = repo.List(value, byField, returnRows, where, pagina, paginaTamanho, ordenacao);
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public Retorno<int> Apagar(TPrimaryKey codigo, bool deleteLogic = false)
        {
            var retorno = new Retorno<int>() { Data = 0 };
            try
            {
                retorno = repo.Delete(codigo, deleteLogic);
            }
            catch (Exception ex)
            {
                retorno.Errors.Add(ex.Message);
            }
            return retorno;
        }

        public bool Existe(string value, string byField)
        {
            return repo.Existe(value, byField);
        }
    }
}