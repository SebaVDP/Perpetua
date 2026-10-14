# Library sample

A small library system whose architecture documentation is generated from attributes in the code. It has six deployment units: Website, Reservations, Loans, Members, Catalog and Notifications.

What the code declares:

- Each deployment unit declares its container with `[Container]` on its `Program` class, in the context `Library`.
- The HTTP dependencies are `[DependsOn]` on the typed clients and controllers: Website calls Reservations and Loans, and Loans calls Members.
- Reservations depends on Catalog with `[DependsOn]`, because it sends the `ReserveCopy` command.
- Loans publishes `BookBorrowed` with `[Event]` on its controller, and the consumers in Catalog, Notifications and Reservations declare `[Consumes]`.

## Generate the documentation

```powershell
.\samples\Library\generate-docs.ps1
```

The script builds the solution and runs `perpetua-structurizr` once per deployment unit. The fragments are written to `samples/Library/docs/Library/`, and git ignores them. Only `samples/Library/docs/workspace.dsl` is written by hand.

## See the diagram

```powershell
.\samples\Library\run-structurizr.ps1
```

This needs Docker. Open <http://localhost:8080> and look at the `Containers` view.

## Run the system

Start the AppHost (needs Docker for RabbitMQ):

```powershell
dotnet run --project samples\Library\Library.AppHost
```

Then send the requests in [`Library.Website.http`](Library.Website/Library.Website.http), from Rider, Visual Studio or VS Code with the REST Client extension. If the port of the website differs, change `@host` at the top of that file.

- `POST /reserve`: Website, then Reservations, which sends the `ReserveCopy` command to Catalog.
- `POST /borrow`: Website, then Loans, which calls Members and publishes `BookBorrowed`.
