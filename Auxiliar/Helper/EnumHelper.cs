using System;

namespace Auxiliar.Helper
{
    public static class EnumHelper
    {
        public static T ToEnum<T>(string valor, T padrao) where T : struct
        {
            if (string.IsNullOrEmpty(valor))
                return padrao;
            T result;
            return Enum.TryParse<T>(valor, true, out result) ? result : padrao;
        }
    }
}
