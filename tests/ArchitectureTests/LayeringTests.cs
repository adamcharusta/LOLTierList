namespace LOLTierList.ArchitectureTests;

public class LayeringTests
{
    public static IEnumerable<object[]> ForbiddenMap =>
    [
        [
            Layers.Domain,
            new[] { Layers.Abstractions, Layers.Application, Layers.Infrastructure, Layers.Web, Layers.Worker }
        ],
        [
            Layers.Abstractions,
            new[] { Layers.Domain, Layers.Application, Layers.Infrastructure, Layers.Web, Layers.Worker }
        ],
        [Layers.Application, new[] { Layers.Infrastructure, Layers.Web, Layers.Worker }],
        [Layers.Infrastructure, new[] { Layers.Application, Layers.Web, Layers.Worker }],
        [Layers.Web, new[] { Layers.Domain, Layers.Abstractions, Layers.Worker }],
        [Layers.Worker, new[] { Layers.Domain, Layers.Web, Layers.Abstractions }]
    ];

    public static IEnumerable<object[]> RequiredMap =>
    [
        [Layers.Application, new[] { Layers.Abstractions, Layers.Domain }],
        [Layers.Infrastructure, new[] { Layers.Abstractions, Layers.Domain }],
        [Layers.Web, new[] { Layers.Application, Layers.Infrastructure }],
        [Layers.Worker, new[] { Layers.Application, Layers.Infrastructure }]
    ];

    [Theory(Skip = "Launch when there is more business logic")]
    [MemberData(nameof(ForbiddenMap))]
    public void Layer_should_NOT_depend_on_forbidden_layers(string assemblyName, string[] forbidden)
    {
        var asm = assemblyName.LoadAssembly();
        if (asm is null || asm.GetTypes().Length == 0)
        {
            return;
        }

        var referencedAssemblies = asm.GetReferencedAssemblies();

        var result = forbidden.Where(x => referencedAssemblies.Any(a => a.Name == x)).ToArray();

        result.Should().BeEmpty(
            $"'{assemblyName}' must NOT depend on ANY of: {string.Join(", ", forbidden)}." +
            (result.Length > 0 ? $" Found: {string.Join(", ", result)}" : string.Empty));
    }

    [Theory(Skip = "Launch when there is more business logic")]
    [MemberData(nameof(RequiredMap))]
    public void Layer_should_depend_on_EACH_required_layer(string assemblyName, string[] required)
    {
        var asm = assemblyName.LoadAssembly();
        if (asm is null || asm.GetTypes().Length == 0)
        {
            return;
        }

        var referencedAssemblies = asm.GetReferencedAssemblies();

        var result = required.Where(x => referencedAssemblies.All(a => a.Name != x)).ToArray();

        result.Should().BeEmpty(
            $"'{assemblyName}' must depend on EACH of: {string.Join(", ", required)}." +
            (result.Length > 0 ? $" Missing: {string.Join(", ", result)}" : string.Empty));
    }
}
