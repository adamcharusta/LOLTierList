namespace LOLTierList.ArchitectureTests;

public class LibraryTests
{
    [Theory]
    [InlineData(Layers.Abstractions)]
    [InlineData(Layers.Domain)]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Web)]
    [InlineData(Layers.Worker)]
    public void Only_Infrastructure_may_reference_Redis_and_Newtonsoft(string asmName)
    {
        var asm = asmName.LoadAssembly();
        if (asm is null)
        {
            return;
        }

        var refs = asm.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
        refs.Should().NotContain(new[] { "StackExchange.Redis", "Newtonsoft.Json" },
            $"Only Infrastructure may reference Redis/Newtonsoft (violated by {asmName})");
    }

    [Theory]
    [InlineData(Layers.Abstractions)]
    [InlineData(Layers.Domain)]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Infrastructure)]
    [InlineData(Layers.Worker)]
    public void Only_Web_may_reference_ASPNET(string asmName)
    {
        var asm = asmName.LoadAssembly();
        if (asm is null)
        {
            return;
        }

        var refs = asm.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
        refs.Should().NotContain(r => r.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal),
            $"{asmName} must not reference Microsoft.AspNetCore.* directly");
    }
}
