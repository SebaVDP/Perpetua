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

    public static Result<Identifier> Create(string value)
        => ValidIdentifier.IsMatch(value)
            ? new Result<Identifier>.Success(new Identifier(value))
            : new Result<Identifier>.Failure();

    public override string ToString() => Value;

    [GeneratedRegex("^[A-Za-z0-9_][A-Za-z0-9_-]*$")]
    private static partial Regex ValidIdentifier { get; }
}
