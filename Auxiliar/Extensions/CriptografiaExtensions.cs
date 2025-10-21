using Auxiliar.Helper;

namespace Auxiliar.Extensions
{
    public static class CriptografiaExtensions
    {
        public static string Encrypt(this string value)
        {
            return AutenticacaoHelper.Encrypt(value);
        }

        public static string Decrypt(this string value)
        {
            return AutenticacaoHelper.Decrypt(value);
        }
    }
}
