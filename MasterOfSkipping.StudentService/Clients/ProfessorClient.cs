using System.Net.Http.Json;
using MasterOfSkipping.StudentService.Contracts.Professor;

namespace MasterOfSkipping.StudentService.Clients;

public sealed class ProfessorClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProfessorClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AskProfessorResponse> AskAsync(
        string subjectCode,
        Guid semesterId,
        int day,
        bool attended,
        CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient(subjectCode);
        var request = new AskProfessorRequest(semesterId, day, attended);

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/ask", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        AskProfessorResponse? result = await response.Content
            .ReadFromJsonAsync<AskProfessorResponse>(cancellationToken);

        return result ?? throw new InvalidOperationException(
            $"Professor {subjectCode} returned an empty response.");
    }
}
