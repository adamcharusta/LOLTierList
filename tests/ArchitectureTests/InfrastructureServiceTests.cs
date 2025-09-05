using System.Reflection;
using System.Runtime.CompilerServices;

namespace LOLTierList.ArchitectureTests;

public class InfrastructureServiceTests
{
    private const string AbstractionsAsmName = Layers.Abstractions;
    private static readonly Assembly Infra = Assembly.Load(Layers.Infrastructure);
    private static readonly string ServiceNamespace = Layers.Infrastructure + ".Services";
    private static readonly string ServiceSuffix = "Service";

    private static bool IsCompilerGenerated(Type t)
    {
        return t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any()
               || t.Name.StartsWith("<")
               || t.FullName?.Contains("+<") == true;
    }

    private static bool IsConcreteServiceType(Type t)
    {
        return t is { IsClass: true, IsAbstract: false }
               && !t.IsNested
               && t.Namespace is not null
               && t.Namespace.StartsWith(ServiceNamespace, StringComparison.Ordinal)
               && !IsCompilerGenerated(t);
    }

    [Fact]
    public void Services_should_be_sealed_and_named_Service()
    {
        var candidates = Infra
            .GetLoadableTypes()
            .Where(IsConcreteServiceType)
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
            .Where(IsConcreteServiceType)
            .ToList();

        services.Should().NotBeEmpty(
            $"expected at least one class under '{ServiceNamespace}'. Loaded: {Infra.Location}");

        var offenders = services
            .Where(t => !t.GetInterfaces().Any(i =>
                // po assembly
                string.Equals(i.Assembly.GetName().Name, AbstractionsAsmName, StringComparison.Ordinal)
                // lub po namespace (gdy assembly name i root namespace się różnią)
                || (i.Namespace?.StartsWith(AbstractionsAsmName, StringComparison.Ordinal) ?? false)))
            .Select(t => t.FullName)
            .OrderBy(x => x)
            .ToArray();

        offenders.Should().BeEmpty(
            $"these classes should implement an interface from {AbstractionsAsmName}: {string.Join(", ", offenders)}");
    }
}
