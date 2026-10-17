# Attributes

Every attribute names the thing it refers to first and its context after it: `[Container(name, context)]`, `[DependsOn(name, context)]` and `[Consumes(eventName, publisherContainer, publisherContext)]`.

## `[Container]`

`[Container(name, context)]` marks a class of a deployment unit as the entry point of the container that unit represents.

```csharp
[Container("Invoicing", "Billing")]
public class InvoicingService { }
```

- The name and the context are required.
- A deployment unit declares **one** container. See [Rules](#rules).

## `[DependsOn]`

`[DependsOn(name, context = null, description = null)]` declares that the container depends on another container. Put it on any class of the same deployment unit.

```csharp
// Billing service
[DependsOn("Website", "Accounts", "Looks up customers")]
public class CustomerLookup { }

[DependsOn("Ledger")]
public class PostingJob { }
```

- The name of the target container is required.
- The context is optional and defaults to the context of the container of the deployment unit (`Billing` here, so `Ledger` is `Billing.Ledger`).
- The description is optional and becomes the description of the relationship.

## `[Publishes]`

`[Publishes(name)]` declares that the container publishes an event. Put it on any class of the deployment unit.

```csharp
// Billing service (container Invoicing, context Billing)
[Publishes("InvoiceIssued")]
public class InvoicingService { }
```

Every published event becomes a container of its own, named `<Container>_<Event>` and tagged `Event`, so two containers can publish an event with the same name.

## `[Consumes]`

`[Consumes(eventName, publisherContainer, publisherContext = null, description = null)]` declares that the container consumes an event published by another container.

```csharp
// Accounts service (container Website, context Accounts)
[Consumes("InvoiceIssued", publisherContainer: "Invoicing", publisherContext: "Billing")]
public class InvoiceInbox { }
```

- The event name and `publisherContainer`, the name of the publishing container, are required.
- `publisherContext` is the context of the publishing container. It is optional and defaults to the context of the consumer.
- The description is optional and becomes the description of the relationship.

The consumer never writes the name of the event container. Perpetua derives it from the publisher and the event.

Both the publisher and the consumer get a relationship to the event, so they never depend on each other directly.

## Rules

- A deployment unit declares **one** container. More than one fails the run and generates nothing; none generates nothing and succeeds.
- A **valid identifier** is one or more of the characters `A-Z`, `a-z`, `0-9`, `_` and `-`, and does not start with `-`. So `Invoice-Api` and `Invoice_Api2` are valid, while `Invoice.Api`, `Invoice Api`, `Invoicé` and the empty string are not. Context, container name, dependency name and context, event name and the publisher and event name of a `[Consumes]` must all be valid identifiers, because they become a folder or a Structurizr identifier. Anything else fails the run.
- A relationship is identified by its target, its context and its description. Declaring the same one twice fails the run, so remove the duplicate. The same target with a different description is allowed, and so is the same container in two contexts.
- A deployment unit may publish an event name only once. The same event name in different deployment units is fine.
- Files from an earlier run with the same context and name are overwritten.

A run that breaks a rule fails and reports a code for each problem. See [Failures](failures.md).

## Breaking API changes

Replace `[Event("Name")]` with `[Publishes("Name")]` (or `EventAttribute` with `PublishesAttribute` in explicit constructor calls). `EventAttribute` has been removed; there is no compatibility alias. Rebuild assemblies with the updated package before scanning them with the updated tool.

For `ConsumesAttribute` constructor calls, rename named arguments `publisher:` to `publisherContainer:` and `context:` to `publisherContext:`. Positional argument order and default-context behavior are unchanged. The public `Publisher` and `Context` properties keep their existing names.

These API changes do not change generated event identifiers, the `Event` tag, relationships or validation rules.

The [Library sample](../samples/Library/README.md) remains pinned to Perpetua 0.1.0-preview.5 and uses its `[Event]` API. Its code and tool version will migrate together when the updated packages are released.
