using Perpetua;

namespace Perpetua.Structurizr;

/// <summary>
/// In the C4 model, a container belongs to a software system; containers with the
/// same context are represented under that system in the generated workspace.
/// </summary>
public sealed class WorkspaceDslWriter
{
    public string Write() => Write([]);

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
