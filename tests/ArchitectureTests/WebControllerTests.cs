using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace LOLTierList.ArchitectureTests;

public class WebControllerTests
{
    private static readonly Assembly Web = Assembly.Load(Layers.Web);

    [Fact]
    public void Mvc_controllers_should_be_public_classes_inheriting_Controller_and_end_with_Controller()
    {
        var controllers = Web.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic)
            .Where(t => t.Name.EndsWith("Controller", StringComparison.Ordinal))
            .Where(t => typeof(Controller).IsAssignableFrom(t)) // MVC (views), not API-only ControllerBase
            .ToArray();

        controllers.Should().NotBeEmpty(
            "there should be at least one MVC controller deriving from Microsoft.AspNetCore.Mvc.Controller");

        var apiDecorated = controllers
            .Where(t => t.GetCustomAttributes(typeof(ApiControllerAttribute), true).Any())
            .Select(t => t.FullName)
            .ToArray();

        apiDecorated.Should().BeEmpty(
            $"MVC controllers should not be decorated with [ApiController] (found: {string.Join(", ", apiDecorated)})");

        foreach (var t in controllers)
        {
            var actions = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => !m.IsDefined(typeof(NonActionAttribute), true))
                .Where(m => m.ReturnType.IsActionReturnType())
                .ToArray();

            actions.Should().NotBeEmpty(
                $"{t.Name} should expose at least one public action returning IActionResult or Task<IActionResult>");
        }
    }

    
}
