using Microsoft.AspNetCore.Mvc;

namespace Perpetua.Structurizr.Tests.FrameworkFixtures;

[Container("Billing", "ReportingService")]
public sealed class ReportingService;

public sealed class FrameworkDependentController : ControllerBase;
