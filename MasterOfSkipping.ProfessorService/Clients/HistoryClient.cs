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
     int day,
     IReadOnlyCollection<string> subjectCodes)
    {
        if (subjectCodes.Count is < 1 or > 2 ||
            subjectCodes.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "One or two subject codes are required.",
                nameof(subjectCodes));
        }

        string query = string.Join(
            "&",
            subjectCodes.Select(subjectCode =>
                $"subjects={Uri.EscapeDataString(subjectCode)}"));

        string path =
            $"/semesters/{semesterId}/days/{day}/questions?{query}";

        List<QuestionResponse>? questions =
            await _httpClient
                .GetFromJsonAsync<List<QuestionResponse>>(path);

        return questions ?? [];
    }



    public async Task SaveLessonAsync(
    Guid semesterId,
    int day,
    SaveLessonRequest request)
    {
        string path =
            $"/semesters/{semesterId}/days/{day}/lessons";

        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(path, request);

        response.EnsureSuccessStatusCode();
    }
}