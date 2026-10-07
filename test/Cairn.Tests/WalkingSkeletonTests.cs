using Cairn;

namespace Cairn.Tests;

public class WalkingSkeletonTests
{
    [Fact]
    public void Generate_emits_a_valid_empty_workspace_dsl()
    {
        var dsl = new Generator().Generate();

        Assert.Equal(
            "workspace {\n    model {\n    }\n    views {\n    }\n}\n",
            dsl);
    }
}
