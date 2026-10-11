using System.Text.RegularExpressions;

namespace Perpetua.Structurizr.Domain;

/// <summary>
/// What a context or container name must look like to be accepted by Structurizr as an identifier
/// and to be usable as a folder name. Structurizr is the stricter of the two: only letters, digits,
/// '_' and '-' are allowed, and not a leading '-'.
/// </summary>
internal static partial class NamingRule
{
    public static bool Accepts(string value) => ValidName.IsMatch(value);

    [GeneratedRegex("^[A-Za-z0-9_][A-Za-z0-9_-]*$")]
    private static partial Regex ValidName { get; }
}
