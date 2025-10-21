using Medo;

namespace Auxiliar.Helper
{
    public static class Uuid7Helper
    {
        public static string Generate()
        {
            return Uuid7.NewUuid7().ToString();
        }
    }
}
