using Microsoft.AspNetCore.Mvc;

namespace Perpetua.Structurizr.Tests.FrameworkFixtures;

[Container("ReportingService", "Billing")]
public sealed class ReportingService;

public sealed class FrameworkDependentController : ControllerBase;
