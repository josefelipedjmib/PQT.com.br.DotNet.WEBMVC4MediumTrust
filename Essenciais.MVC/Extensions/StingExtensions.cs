
namespace Essenciais.MVC.Extensions
{
    public static class StingExtensions
    {
        public static string RemoveFinalSlash(this string value)
        {
            return value.TrimEnd('/');
        }

        public static string CapitalizeFirstLetter(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}