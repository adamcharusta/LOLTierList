using System.Reflection;

namespace LOLTierList.ArchitectureTests;

public class PackageBoundaryTests
{
    private static string[] Refs(string asm)
    {
        return Assembly.Load(asm).GetReferencedAssemblies().Select(a => a.Name!).ToArray();
    }

    [Theory]
    [InlineData(Layers.Abstractions)]
    [InlineData(Layers.Domain)]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Web)]
    [InlineData(Layers.Worker)]
    public void Only_Infrastructure_may_reference_Redis_and_Newtonsoft(string asm)
    {
        var refs = Refs(asm);
        refs.Should().NotContain(["StackExchange.Redis", "Newtonsoft.Json"],
            $"only Infrastructure may reference Redis/Newtonsoft (violated by {asm})");
    }

    [Theory]
    [InlineData(Layers.Abstractions)]
    [InlineData(Layers.Domain)]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Infrastructure)]
    [InlineData(Layers.Worker)]
    public void Only_Web_may_reference_ASPNET(string asm)
    {
        var refs = Refs(asm);
        refs.Should().NotContain(r => r.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal),
            $"{asm} must not reference Microsoft.AspNetCore.* directly");
    }
}
