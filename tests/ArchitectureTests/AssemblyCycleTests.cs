using System.Reflection;

namespace LOLTierList.ArchitectureTests;

public class AssemblyCycleTests
{
    private static readonly string[] Nodes =
    {
        Layers.Abstractions, Layers.Domain, Layers.Infrastructure, Layers.Application, Layers.Web, Layers.Worker
    };

    [Fact]
    public void There_should_be_no_cycles_between_assemblies()
    {
        var graph = Nodes.ToDictionary(n => n, n =>
            Assembly.Load(n).GetReferencedAssemblies().Select(a => a.Name!).Where(Nodes.Contains).ToArray());

        bool Dfs(string node, string start, HashSet<string> seen)
        {
            foreach (var next in graph[node])
            {
                if (next == start)
                {
                    return true;
                }

                if (seen.Add(next) && Dfs(next, start, seen))
                {
                    return true;
                }
            }

            return false;
        }

        var cycles = new List<string>();
        foreach (var n in Nodes)
        {
            if (Dfs(n, n, new HashSet<string>()))
            {
                cycles.Add(n);
            }
        }

        cycles.Should().BeEmpty("there must be no reference cycles between layers");
    }
}
