using System.Net.Http.Json;
using Library.Loans.Contracts;

namespace Library.Website.Clients;

// Typed client for the Loans service. Service discovery resolves "http://loans".
[Perpetua.DependsOn("Loans", description: "Borrows a book")]
public class LoansClient
{
    private readonly HttpClient _http;

    public LoansClient(HttpClient http) => _http = http;

    public async Task<BorrowResponse?> BorrowAsync(Guid memberId, Guid bookId, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/loans", new BorrowRequest(memberId, bookId), ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BorrowResponse>(ct);
    }
}
