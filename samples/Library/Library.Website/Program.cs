using Library.Website.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Typed clients; service discovery resolves "http://reservations" and "http://loans".
builder.Services.AddHttpClient<ReservationsClient>(c => c.BaseAddress = new Uri("http://reservations"));
builder.Services.AddHttpClient<LoansClient>(c => c.BaseAddress = new Uri("http://loans"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/reserve", (ReservationsClient reservations) => reservations.ReserveAsync(Guid.NewGuid(), Guid.NewGuid()));
app.MapPost("/borrow", (LoansClient loans) => loans.BorrowAsync(Guid.NewGuid(), Guid.NewGuid()));

app.Run();

[Perpetua.Container("Website", "Library")]
public partial class Program;
