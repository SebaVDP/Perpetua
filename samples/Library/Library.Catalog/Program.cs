using MassTransit;
using Library.Catalog;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<ReserveCopyConsumer>();
    x.AddConsumer<MarkCopyBorrowedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        if (!string.IsNullOrEmpty(rabbitConnection))
        {
            cfg.Host(new Uri(rabbitConnection));
        }
        else
        {
            cfg.Host("localhost", "/", h => { h.Username("guest"); h.Password("guest"); });
        }
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();

[Perpetua.Container("Catalog", "Library")]
public partial class Program;
