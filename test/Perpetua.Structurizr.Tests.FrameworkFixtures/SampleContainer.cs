using Microsoft.AspNetCore.Mvc;

namespace Perpetua.Structurizr.Tests.FrameworkFixtures;

[Container("SampleSystem", "FrameworkIndependentContainer")]
public sealed class SampleContainer;

public sealed class FrameworkDependentController : ControllerBase;
