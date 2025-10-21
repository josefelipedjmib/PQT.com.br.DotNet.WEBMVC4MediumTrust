using Auxiliar.DataAnnotationCustoms;
using System.ComponentModel;

namespace Auxiliar.Enums
{
    public enum TipoLoginEnum
    {
        [Description("Admin")]
        [Ativo(true)]
        Admin = 1,
        [Description("Membro")]
        [Ativo(true)]
        Membro = 2,
        [Description("Usuario")]
        [Ativo(true)]
        Usuario = 3
    }
}
