using MasterOfSkipping.ProfessorService.Clients;
using MasterOfSkipping.ProfessorService.Contracts.History;
using MasterOfSkipping.ProfessorService.Contracts.Messaging;
using MasterOfSkipping.ProfessorService.Contracts.Student;
using MasterOfSkipping.ProfessorService.Services;

namespace MasterOfSkipping.ProfessorService.Endpoints;

public static class ProfessorEndpoints
{
    public static void MapProfessorEndpoints(
        this WebApplication app,
        string subjectCode)
    {
        app.MapPost("/ask", async (
    StudentRequest request,
    HistoryClient historyClient,
    ProfessorRule professorRule,
    LessonResultPublisher publisher,
    CancellationToken cancellationToken) =>
{
    if (request.Day is < 1 or > 100)
    {
        return Results.BadRequest(new
        {
            Error = "Day must be between 1 and 100."
        });
    }
    ProfessorSettingResponse? setting =
    await historyClient.GetSettingAsync(
        request.SemesterId,
        subjectCode);
    if (setting is null)
    {
        return Results.NotFound(new
        {
            Error = "Professor setting was not found."
        });
    }
    string[] requiredSubjects =
    professorRule.GetRequiredSubjects(setting);
    IReadOnlyCollection<QuestionResponse> questions = [];
    if (request.Day > 1 && requiredSubjects.Length > 0)
    {
        questions = await historyClient.GetQuestionsAsync(
            request.SemesterId,
            request.Day - 1,
            requiredSubjects);
    }

    bool wasAsked = professorRule.ShouldAsk(
    request.Day, setting, questions);

    var message = new LessonResultMessage(
        Guid.NewGuid(),
        request.SemesterId,
        request.Day,
        subjectCode,
        request.Attended,
        wasAsked);

    await publisher.PublishAsync(message, cancellationToken);

    var response = new StudentResponse(subjectCode, wasAsked);

    return Results.Ok(response);
    
});
    }
}
