using System.Reflection;

namespace LOLTierList.ArchitectureTests.Common;

public static class AssemblyExtensions
{
    public static Assembly? LoadAssembly(this string name)
    {
        try
        {
            return AppDomain.CurrentDomain
                       .GetAssemblies()
                       .FirstOrDefault(a => a.GetName().Name == name)
                   ?? Assembly.Load(name);
        }
        catch { return null; }
    }

    public static IReadOnlyList<Type> GetLoadableTypes(this Assembly asm)
    {
        try
        {
            return asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types?.OfType<Type>().ToArray()
                   ?? Array.Empty<Type>();
        }
    }
}
