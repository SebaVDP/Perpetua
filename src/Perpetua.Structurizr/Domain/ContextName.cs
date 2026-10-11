namespace Perpetua.Structurizr.Domain;

/// <summary>The name of a software-system context, kept distinct from a container name so the two cannot be mixed up.</summary>
public sealed record ContextName
{
    private ContextName(string value) => Value = value;

    public string Value { get; }

    public static Result<ContextName> Create(string value)
        => NamingRule.Accepts(value)
            ? new Result<ContextName>.Success(new ContextName(value))
            : new Result<ContextName>.Failure();

    public override string ToString() => Value;
}
