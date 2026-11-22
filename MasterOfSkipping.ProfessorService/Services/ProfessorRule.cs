using MasterOfSkipping.ProfessorService.Contracts.History;

namespace MasterOfSkipping.ProfessorService.Services;

public sealed class ProfessorRule
{
    public string[] GetRequiredSubjects(
        ProfessorSettingResponse setting)
    {
        string[] requiredSubjects;
        switch (setting.RuleCode)
        {
            case "RANDOM_50":
                requiredSubjects = [];
                break;

            case "YESTERDAY_A":
                if (setting.DependencyA is null)
                {
                    throw new InvalidOperationException(
                        "YESTERDAY_A requires DependencyA.");
                }

                requiredSubjects = [setting.DependencyA];
                break;

            case "XOR":
                if (setting.DependencyA is null ||
                    setting.DependencyB is null)
                {
                    throw new InvalidOperationException(
                        "XOR requires DependencyA and DependencyB.");
                }

                requiredSubjects =
                [
                    setting.DependencyA, setting.DependencyB
                ];
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown rule: {setting.RuleCode}");
        }


        return requiredSubjects;
    }

    public bool ShouldAsk(
        int day,
        ProfessorSettingResponse setting,
        IReadOnlyCollection<QuestionResponse> questions)
    {
        string[] requiredSubjects =
            GetRequiredSubjects(setting);

        bool wasAsked;

        switch (setting.RuleCode)
        {
            case "RANDOM_50":
                int random = Random.Shared.Next(2);
                wasAsked = random == 1;
                break;

            case "YESTERDAY_A":
                wasAsked = WasAskedYesterday(
                    day,
                    requiredSubjects[0],
                    questions);
                break;

            case "XOR":
                bool askedA = WasAskedYesterday(
                    day,
                    requiredSubjects[0],
                    questions);

                bool askedB = WasAskedYesterday(
                    day,
                    requiredSubjects[1],
                    questions);

                wasAsked = askedA ^ askedB;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown rule: {setting.RuleCode}");
        }

        return wasAsked;
    }

    private static bool WasAskedYesterday(
    int day,
    string subjectCode,
    IReadOnlyCollection<QuestionResponse> questions)
    {
        if (day == 1)
        {
            return false;
        }

        QuestionResponse? question = questions
            .SingleOrDefault(question =>
                question.SubjectCode == subjectCode);

        if (question is null)
        {
            throw new InvalidOperationException(
                $"Question result for {subjectCode} was not found.");
        }

        return question.WasAsked;
    }
}
