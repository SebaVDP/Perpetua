namespace Perpetua.Structurizr.Domain;

/// <summary>The name of a container, kept distinct from a context name so the two cannot be mixed up.</summary>
public sealed record ContainerName
{
    private ContainerName(string value) => Value = value;

    public string Value { get; }

    public static Result<ContainerName> Create(string value)
        => NamingRule.Accepts(value)
            ? new Result<ContainerName>.Success(new ContainerName(value))
            : new Result<ContainerName>.Failure();

    public override string ToString() => Value;
}
