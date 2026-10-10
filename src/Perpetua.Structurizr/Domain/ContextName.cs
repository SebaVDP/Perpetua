namespace Perpetua.Structurizr.Domain;

/// <summary>The name of a software-system context, kept distinct from a container name so the two cannot be mixed up.</summary>
public sealed record ContextName
{
    private ContextName(Identifier identifier) => Identifier = identifier;

    public Identifier Identifier { get; }

    public string Value => Identifier.Value;

    public static Result<ContextName> Create(string value)
        => Identifier.Create(value).Map(identifier => new ContextName(identifier));

    public override string ToString() => Value;
}
