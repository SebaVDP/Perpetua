namespace Perpetua.Structurizr.Domain;

/// <summary>The name of a container, kept distinct from a context name so the two cannot be mixed up.</summary>
public sealed record ContainerName
{
    private ContainerName(Identifier identifier) => Identifier = identifier;

    public Identifier Identifier { get; }

    public string Value => Identifier.Value;

    public static Result<ContainerName> Create(string value)
        => Identifier.Create(value).Map(identifier => new ContainerName(identifier));

    public override string ToString() => Value;
}
