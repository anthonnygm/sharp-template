using System.Text;

namespace Appel.SharpTemplate.API.Extensions;

public static class StringExtensions
{
    public static string ToNumbersOnly(this string value)
    {
        var newValue = new StringBuilder();

        foreach (var i in value)
        {
            newValue.Append(char.IsNumber(i) ? i.ToString() : string.Empty);
        }

        return newValue.ToString();
    }
}
