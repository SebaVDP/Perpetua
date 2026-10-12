using Perpetua.Structurizr.Domain;
using Perpetua.Structurizr.Infrastructure;
using static Perpetua.Structurizr.Tests.HandlerTestSupport;

namespace Perpetua.Structurizr.Tests;

public class DeploymentUnitDslWriterTests
{
    [Fact]
    public void Writes_each_published_event_as_a_container_tagged_Event_next_to_the_container_of_the_unit()
    {
        // Given
        var outputDirectory = Directory.CreateTempSubdirectory();
        var writer = new DeploymentUnitDslWriter(outputDirectory);
        var unit = new DeploymentUnitAggregate(Context("Billing"), Container("InvoiceApi"))
        {
            PublishedEvents = [new PublishedEvent(Container("InvoiceApi_OrderPlaced"))]
        };

        try
        {
            // When
            writer.Write(unit);

            // Then
            Assert.Equal(
                """
                InvoiceApi = container "InvoiceApi"
                InvoiceApi_OrderPlaced = container "InvoiceApi_OrderPlaced" "" "" "Event"

                """.ReplaceLineEndings("\n"),
                File.ReadAllText(Path.Combine(outputDirectory.FullName, "Billing", "containers", "InvoiceApi.dsl")));
        }
        finally
        {
            outputDirectory.Delete(recursive: true);
        }
    }
}
