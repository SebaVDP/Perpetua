using Perpetua;

namespace Perpetua.Structurizr;

/// <summary>
/// Writes Structurizr workspace DSL. The walking-skeleton implementation emits
/// the canonical empty workspace that downstream tooling can render.
/// </summary>
public sealed class WorkspaceDslWriter
{
    /// <summary>Returns a valid, empty Structurizr <c>workspace.dsl</c> document.</summary>
    public string Write() => Write([]);

    /// <summary>Returns a workspace containing the declared software systems and containers.</summary>
    public string Write(IEnumerable<ContainerAttribute> containers)
    {
        var lines = new List<string>
        {
            "workspace {",
            "    model {"
        };

        foreach (var group in containers.GroupBy(container => container.Context).OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            lines.Add($"        softwareSystem \"{group.Key}\" {{");
            lines.AddRange(group.OrderBy(container => container.Name, StringComparer.Ordinal).Select(container => $"            container \"{container.Name}\""));
            lines.Add("        }");
        }

        lines.Add("    }");
        lines.Add("    views {");
        lines.Add("    }");
        lines.Add("}");

        return $"{string.Join('\n', lines)}\n";
    }
}
