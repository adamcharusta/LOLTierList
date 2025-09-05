using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace LOLTierList.ArchitectureTests.Common;

public static class TypeExtensions
{
    private static bool IsCompilerGenerated(this Type t)
    {
        return t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any()
               || t.Name.StartsWith("<")
               || t.FullName?.Contains("+<") == true;
    }

    public static bool IsConcreteServiceType(this Type t, string serviceNamespace)
    {
        return t is { IsClass: true, IsAbstract: false }
               && !t.IsNested
               && t.Namespace is not null
               && t.Namespace.StartsWith(serviceNamespace, StringComparison.Ordinal)
               && !IsCompilerGenerated(t);
    }

    public static bool IsActionReturnType(this Type t)
    {
        if (typeof(IActionResult).IsAssignableFrom(t))
        {
            return true;
        }

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>)
                            && typeof(IActionResult).IsAssignableFrom(t.GetGenericArguments()[0]))
        {
            return true;
        }

        if (t == typeof(Task))
        {
            return true;
        }

        return false;
    }
}
