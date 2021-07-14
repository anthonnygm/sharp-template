using System.Text;

namespace Appel.SharpTemplate.Utils
{
    internal static class StringExtensions
    {
        public static string ToNumbersOnly(this string value)
        {
            var newValue = new StringBuilder();

            for (var i = 0; i < value.Length; i++)
            {
                newValue.Append(char.IsNumber(value[i]) ? value[i].ToString() : string.Empty);
            }

            return newValue.ToString();
        }
    }
}
