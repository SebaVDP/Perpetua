using System.Net.Http.Json;
using Library.Reservations.Contracts;

namespace Library.Website.Clients;

// Typed client for the Reservations service. Service discovery resolves "http://reservations".
[Perpetua.DependsOn("Reservations", description: "Reserves a book")]
public class ReservationsClient
{
    private readonly HttpClient _http;

    public ReservationsClient(HttpClient http) => _http = http;

    public async Task<ReserveResponse?> ReserveAsync(Guid memberId, Guid titleId, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/reservations", new ReserveRequest(memberId, titleId), ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReserveResponse>(ct);
    }
}
