namespace Perpetua.Structurizr.Domain;

public sealed record ContextDslFragment(string ContextName, ContainerDslFragment Container)
    : ContainerDslFragmentsResult;
