using System.Net.Http.Json;
using MasterOfSkipping.StudentService.Contracts.History;

namespace MasterOfSkipping.StudentService.Clients;

public sealed class HistoryClient
{
    private readonly HttpClient _httpClient;

    public HistoryClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("HistoryService");
    }

    public async Task<CreateSemesterResponse> CreateSemesterAsync(
        int? seed,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateSemesterRequest(seed);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/semesters", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        CreateSemesterResponse? semester = await response.Content
            .ReadFromJsonAsync<CreateSemesterResponse>(cancellationToken);

        return semester ?? throw new InvalidOperationException(
            "History Service returned an empty semester response.");
    }

    public async Task StartDayAsync(
        Guid semesterId,
        int day,
        CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.PostAsync(
            $"/semesters/{semesterId}/days/{day}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<HistoryLessonResponse>> GetHistoryAsync(
        Guid semesterId,
        int beforeDay,
        CancellationToken cancellationToken = default)
    {
        List<HistoryLessonResponse>? history = await _httpClient
            .GetFromJsonAsync<List<HistoryLessonResponse>>(
                $"/semesters/{semesterId}/history?beforeDay={beforeDay}",
                cancellationToken);

        return history ?? [];
    }

    public async Task<SemesterStateResponse> CompleteDayAsync(
        Guid semesterId,
        int day,
        int totalEnjoyment,
        bool isExpelled,
        CancellationToken cancellationToken = default)
    {
        var request = new CompleteDayRequest(totalEnjoyment, isExpelled);
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
            $"/semesters/{semesterId}/days/{day}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        SemesterStateResponse? semester = await response.Content
            .ReadFromJsonAsync<SemesterStateResponse>(cancellationToken);

        return semester ?? throw new InvalidOperationException(
            "History Service returned an empty day-completion response.");
    }
}
