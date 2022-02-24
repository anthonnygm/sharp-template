using FluentValidation.Internal;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Appel.SharpTemplate.Infrastructure.Application;

public class CamelCasePropertyNameResolver
{
    public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
        return ToCamelCase(DefaultPropertyNameResolver(type, memberInfo, expression));
    }

    private static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
        if (expression != null)
        {
            var chain = PropertyChain.FromExpression(expression);

            if (chain.Count > 0)
            {
                return chain.ToString();
            }
        }

        return memberInfo?.Name;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
        {
            return value;
        }

        var chars = value.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = (i + 1 < chars.Length);

            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
        }

        return new string(chars);
    }
}
