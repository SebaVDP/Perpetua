using MassTransit;
using Library.Reservations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<CloseReservationConsumer>();
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

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapControllers();

app.Run();

[Perpetua.Container("Reservations", "Library")]
public partial class Program;
