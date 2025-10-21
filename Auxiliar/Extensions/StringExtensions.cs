
namespace Auxiliar.Extensions
{
    public static class SringExtensions
    {
        public static string RemoveFinalSlash(this string value)
        {
            return value.TrimEnd('/');
        }

        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
