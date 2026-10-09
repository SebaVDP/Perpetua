namespace Perpetua.Structurizr.Domain;

/// <summary>
/// The container of a deployment unit together with the software-system context it belongs to.
/// A context holds exactly one container per run, because each deployment unit runs the tool separately.
/// </summary>
public sealed record ContextDslFragment(string ContextName, ContainerDslFragment Container);
