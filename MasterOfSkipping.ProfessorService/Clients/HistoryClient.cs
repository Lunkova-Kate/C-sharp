namespace MasterOfSkipping.ProfessorService.Clients;

using MasterOfSkipping.ProfessorService.Contracts.History;

public sealed class HistoryClient
{
    private readonly HttpClient _httpClient;

    public HistoryClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient =
            httpClientFactory.CreateClient("HistoryService");
    }


    public async Task<ProfessorSettingResponse?> GetSettingAsync(
    Guid semesterId,
    string subjectCode)
    {
        string path =
            $"/semesters/{semesterId}/settings/{subjectCode}";

        ProfessorSettingResponse? setting =
            await _httpClient
                .GetFromJsonAsync<ProfessorSettingResponse>(path);

        return setting;
    }


    public async Task<List<QuestionResponse>> GetQuestionsAsync(
    Guid semesterId,
    int day)
    {
        string path =
            $"/semesters/{semesterId}/days/{day}/questions";

        List<QuestionResponse>? questions =
            await _httpClient
                .GetFromJsonAsync<List<QuestionResponse>>(path);

        return questions ?? [];
    }
}