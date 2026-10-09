namespace Perpetua.Structurizr.Domain;

public sealed record ContextDslFragment(string ContextName, List<ContainerDslFragment> Containers)
    : ContainerDslFragmentsResult;
