using System.Xml.Linq;

namespace LOLTierList.ArchitectureTests;

public class ProjectPolicyTests
{
    private static string Root()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !File.Exists(Path.Combine(d.FullName, "LOLTierList.sln")))
        {
            d = d.Parent;
        }

        return d!.FullName;
    }

    private static XDocument? LoadIfExists(string path)
    {
        return File.Exists(path) ? XDocument.Load(path) : null;
    }

    private static string? GetProperty(XDocument? doc, string propName)
    {
        return doc?.Descendants()
            .Where(e => e.Name.LocalName == propName)
            .Select(e => (string?)e.Value)
            .LastOrDefault();
    }

    [Theory]
    [InlineData("src/Abstractions/Abstractions.csproj")]
    [InlineData("src/Domain/Domain.csproj")]
    [InlineData("src/Application/Application.csproj")]
    [InlineData("src/Infrastructure/Infrastructure.csproj")]
    [InlineData("src/Web/Web.csproj")]
    [InlineData("src/Worker/Worker.csproj")]
    public void Csproj_should_enable_nullable_and_warn_as_error(string rel)
    {
        var root = Root();
        var csprojPath = Path.Combine(root, rel);
        var propsPath = Path.Combine(root, "Directory.Build.props");

        var csproj = LoadIfExists(csprojPath) ?? throw new FileNotFoundException(csprojPath);
        var props = LoadIfExists(propsPath);

        var nullable = GetProperty(csproj, "Nullable") ?? GetProperty(props, "Nullable");
        var two = GetProperty(csproj, "TreatWarningsAsErrors") ?? GetProperty(props, "TreatWarningsAsErrors");

        nullable.Should().NotBeNull(
            $"set <Nullable> in {rel} or in Directory.Build.props");

        nullable!.Should().BeOneOf("enable", "annotations", "warnings",
            $"<Nullable> must be 'enable', 'annotations', or 'warnings' (found '{nullable}') in {rel} or Directory.Build.props");

        two.Should().NotBeNull(
            $"set <TreatWarningsAsErrors> in {rel} or in Directory.Build.props");

        two!.Should().Be("true",
            $"<TreatWarningsAsErrors> must be 'true' (found '{two}') in {rel} or Directory.Build.props");
    }
}
