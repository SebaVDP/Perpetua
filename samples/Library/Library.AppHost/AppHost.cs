var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var members = builder.AddProject<Projects.Library_Members>("members");

var loans = builder.AddProject<Projects.Library_Loans>("loans")
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithReference(members);

var reservations = builder.AddProject<Projects.Library_Reservations>("reservations")
    .WithReference(rabbitmq).WaitFor(rabbitmq);

builder.AddProject<Projects.Library_Website>("website")
    .WithReference(reservations)
    .WithReference(loans)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.Library_Catalog>("catalog")
    .WithReference(rabbitmq).WaitFor(rabbitmq);

builder.AddProject<Projects.Library_Notifications>("notifications")
    .WithReference(rabbitmq).WaitFor(rabbitmq);

builder.Build().Run();
