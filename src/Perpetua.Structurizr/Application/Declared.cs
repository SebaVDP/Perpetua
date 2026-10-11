namespace Perpetua.Structurizr.Application;

/// <summary>An attribute together with the full name of the class it is declared on.</summary>
public sealed record Declared<T>(T Attribute, string DeclaringClass);
