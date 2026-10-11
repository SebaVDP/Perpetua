namespace Perpetua.Structurizr.Domain;

/// <summary>An attribute together with the class it is declared on.</summary>
public sealed record Declared<T>(T Attribute, DeclaringClass DeclaringClass);
