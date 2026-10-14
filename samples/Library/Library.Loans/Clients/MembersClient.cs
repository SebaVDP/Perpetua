using System.Net.Http.Json;
using Library.Members.Contracts;

namespace Library.Loans.Clients;

// Typed client for the Members service. Service discovery resolves "http://members".
[Perpetua.DependsOn("Members", description: "Checks borrowing rights")]
public class MembersClient
{
    private readonly HttpClient _http;

    public MembersClient(HttpClient http) => _http = http;

    public async Task<BorrowingRightsResponse?> CheckRightsAsync(BorrowingRightsRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/members/check-rights", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BorrowingRightsResponse>(ct);
    }
}
