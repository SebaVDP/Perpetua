using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Perpetua.Structurizr.Domain;

/// <summary>
/// A name that Structurizr accepts as an identifier and that is also usable as a folder name.
/// Structurizr is the stricter of the two: only letters, digits, '_' and '-' are allowed, and not a leading '-'.
/// </summary>
public sealed partial record Identifier
{
    private Identifier(string value) => Value = value;

    public string Value { get; }

    public static bool TryCreate(string value, [NotNullWhen(true)] out Identifier? identifier)
    {
        identifier = ValidIdentifier.IsMatch(value) ? new Identifier(value) : null;
        return identifier is not null;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[A-Za-z0-9_][A-Za-z0-9_-]*$")]
    private static partial Regex ValidIdentifier { get; }
}
