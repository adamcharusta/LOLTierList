using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace LOLTierList.ArchitectureTests;

public class WorkerTests
{
    private static readonly Assembly WorkerAsm = Assembly.Load(Layers.Worker);
    private static readonly string WorkerName = "Worker";

    [Fact]
    public void Worker_should_have_single_BackgroundService()
    {
        var types = WorkerAsm.GetTypes()
            .Where(t => typeof(BackgroundService).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();

        types.Should().ContainSingle("there should be exactly one BackgroundService implementation");
        types.Single().Name.Should().Be(WorkerName, $"the background service class should be named '{WorkerName}'");
    }
}
