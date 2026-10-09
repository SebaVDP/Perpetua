namespace Perpetua.Structurizr;

public sealed record ContextDslFragment(string ContextName, List<ContainerDslFragment> Containers)
    : ContainerDslFragmentsResult;
