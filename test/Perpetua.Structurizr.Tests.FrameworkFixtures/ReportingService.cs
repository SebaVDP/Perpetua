using Microsoft.AspNetCore.Mvc;

namespace Perpetua.Structurizr.Tests.FrameworkFixtures;

[Container("ReportingService", "Billing")]
public sealed class ReportingService;

[Publishes("ReportGenerated")]
[DependsOn("Catalog", "Library")]
[Consumes("BookBorrowed", publisherContainer: "Loans", publisherContext: "Library")]
public sealed class FrameworkDependentController : ControllerBase;

public sealed class ReportingModule
{
    [Container("ReportingExporter", "Billing")]
    public sealed class Exporter;
}

public sealed class MarkerAttribute : Attribute;

[Marker]
[Container("MarkedService", "Billing")]
public sealed class MarkedService;
