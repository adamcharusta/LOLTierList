using System.Reflection;

namespace LOLTierList.ArchitectureTests;

public class InfrastructureServiceTests
{
    private const string AbstractionsAsmName = Layers.Abstractions;
    private static readonly Assembly Infra = Assembly.Load(Layers.Infrastructure);
    private static readonly string ServiceNamespace = Layers.Infrastructure + ".Services";
    private static readonly string ServiceSuffix = "Service";

    [Fact]
    public void Services_should_be_sealed_and_named_Service()
    {
        var candidates = Infra
            .GetLoadableTypes()
            .Where(x => x.IsConcreteServiceType(ServiceNamespace))
            .ToList();

        candidates.Should().NotBeEmpty(
            $"expected at least one class under '{ServiceNamespace}'. Loaded: {Infra.Location}");

        var notSealed = candidates
            .Where(t => !t.IsSealed)
            .Select(t => t.FullName)
            .OrderBy(x => x)
            .ToArray();

        var wrongName = candidates
            .Where(t => !t.Name.EndsWith(ServiceSuffix, StringComparison.Ordinal))
            .Select(t => t.FullName)
            .OrderBy(x => x)
            .ToArray();

        notSealed.Should().BeEmpty(
            $"these classes should be 'sealed': {string.Join(", ", notSealed)}");

        wrongName.Should().BeEmpty(
            $"these classes should end with '{ServiceSuffix}': {string.Join(", ", wrongName)}");
    }

    [Fact]
    public void Services_should_implement_interface_from_Abstractions()
    {
        var services = Infra
            .GetLoadableTypes()
            .Where(x => x.IsConcreteServiceType(ServiceNamespace))
            .ToList();

        services.Should().NotBeEmpty(
            $"expected at least one class under '{ServiceNamespace}'. Loaded: {Infra.Location}");

        var offenders = services
            .Where(t => !t.GetInterfaces().Any(i =>
                string.Equals(i.Assembly.GetName().Name, AbstractionsAsmName, StringComparison.Ordinal)
                || (i.Namespace?.StartsWith(AbstractionsAsmName, StringComparison.Ordinal) ?? false)))
            .Select(t => t.FullName)
            .OrderBy(x => x)
            .ToArray();

        offenders.Should().BeEmpty(
            $"these classes should implement an interface from {AbstractionsAsmName}: {string.Join(", ", offenders)}");
    }
}
