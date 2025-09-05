using System.Reflection;
using System.Runtime.CompilerServices;

namespace LOLTierList.ArchitectureTests;

public class AsyncVoidTests
{
    [Theory]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Infrastructure)]
    [InlineData(Layers.Web)]
    [InlineData(Layers.Worker)]
    public void No_async_void_methods(string asmName)
    {
        var asm = Assembly.Load(asmName);
        var offenders = asm.GetTypes()
            .SelectMany(t =>
                t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                             BindingFlags.NonPublic))
            .Where(m => m.ReturnType == typeof(void))
            .Where(m => m.GetCustomAttribute<AsyncStateMachineAttribute>() is not null)
            .Select(m => $"{m.DeclaringType!.FullName}.{m.Name}")
            .ToArray();

        offenders.Should().BeEmpty("avoid 'async void' methods (except event handlers)");
    }
}
