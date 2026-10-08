namespace Perpetua.Structurizr.Tests;

public class WorkspaceDslWriterTests
{
    [Fact]
    public void Writes_each_container_under_its_software_system()
    {
        var dsl = new WorkspaceDslWriter().Write(
        [
            new ContainerAttribute("ExampleSystem", "Website"),
        ]);

        Assert.Equal(
            """
            workspace {
                model {
                    softwareSystem "ExampleSystem" {
                        container "Website"
                    }
                }
                views {
                }
            }

            """.ReplaceLineEndings("\n"),
            dsl);
    }

    [Fact]
    public void Orders_software_systems_and_their_containers_alphabetically()
    {
        var dsl = new WorkspaceDslWriter().Write(
        [
            new ContainerAttribute("Billing", "Worker"),
            new ContainerAttribute("Accounts", "Website"),
            new ContainerAttribute("Billing", "Api"),
            new ContainerAttribute("Accounts", "Database"),
        ]);

        Assert.Equal(
            """
            workspace {
                model {
                    softwareSystem "Accounts" {
                        container "Database"
                        container "Website"
                    }
                    softwareSystem "Billing" {
                        container "Api"
                        container "Worker"
                    }
                }
                views {
                }
            }

            """.ReplaceLineEndings("\n"),
            dsl);
    }
}
