using Perpetua.Structurizr.Domain;

namespace Perpetua.Structurizr.Application;

/// <summary>Where the generated container DSL goes.</summary>
public interface IContainerDslOutput
{
    void Write(ContextDeclaration context);
}
