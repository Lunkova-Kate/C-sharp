namespace MasterOfSkipping.ProfessorService.Clients;

public sealed class HistoryClient
{
    private readonly HttpClient _httpClient;

    public HistoryClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient =
            httpClientFactory.CreateClient("HistoryService");
    }
}