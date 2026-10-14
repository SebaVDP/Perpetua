using MassTransit;
using Library.Loans.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
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

// Typed client; service discovery resolves "http://members" to the real endpoint.
builder.Services.AddHttpClient<MembersClient>(c => c.BaseAddress = new Uri("http://members"));

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapControllers();

app.Run();

[Perpetua.Container("Loans", "Library")]
public partial class Program;
