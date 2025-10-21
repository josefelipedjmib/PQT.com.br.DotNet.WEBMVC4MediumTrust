using System;
using System.ComponentModel.DataAnnotations;

namespace Auxiliar.DataAnnotationCustoms
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AtivoAttribute : ValidationAttribute
    {
        public bool Ativo { get; }

        public AtivoAttribute(bool ativo)
        {
            Ativo = ativo;
        }
    }
}