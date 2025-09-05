using System.Reflection;

namespace LOLTierList.ArchitectureTests;

public class NamespaceConsistencyTests
{
    [Theory]
    [InlineData(Layers.Abstractions)]
    [InlineData(Layers.Domain)]
    [InlineData(Layers.Application)]
    [InlineData(Layers.Infrastructure)]
    [InlineData(Layers.Web)]
    [InlineData(Layers.Worker)]
    public void Public_types_should_start_with_root_namespace(string layer)
    {
        var asm = Assembly.Load(layer);
        var offenders = asm.GetExportedTypes()
            .Where(t => t.Namespace is not null && !t.Namespace.StartsWith(layer, StringComparison.Ordinal))
            .Select(t => t.FullName)
            .ToArray();

        offenders.Should().BeEmpty(
            $"public types in {layer} should have a namespace starting with '{layer}'");
    }
}
