using MasterOfSkipping.HistoryService.Data.Entities;


namespace MasterOfSkipping.HistoryService.Services;


public sealed class ProfessorSettingsGenerator
{
    public List<SubjectSettingEntity> Generate(
        Guid semesterId,
        int seed)
    {
        string[] subjectCodes =
        [
            "CALCULUS",
            "LINEAR_ALGEBRA",
            "DISCRETE_MATH",
            "PROGRAMMING",
            "PHYSICS",
            "ENGLISH"
        ];

        string[] ruleCodes =
        [
            "RANDOM_50",
            "YESTERDAY_A",
            "XOR"
        ];


        var settings = new List<SubjectSettingEntity>();

        var random = new Random(seed);

        foreach (string subjectCode in subjectCodes)
        {
            int ruleIndex = random.Next(ruleCodes.Length);
            string ruleCode = ruleCodes[ruleIndex];
            string? dependencyA = null;
            string? dependencyB = null;

            if (ruleCode == "YESTERDAY_A")
            {
                int dependencyIndex = random.Next(subjectCodes.Length);
                dependencyA = subjectCodes[dependencyIndex];
            }
            else if (ruleCode == "XOR")
            {
                int firstIndex = random.Next(subjectCodes.Length);
                dependencyA = subjectCodes[firstIndex];
                int secondIndex;

                do
                {
                    secondIndex = random.Next(subjectCodes.Length);
                }
                while (secondIndex == firstIndex);

                dependencyB = subjectCodes[secondIndex];
            }
            var setting = new SubjectSettingEntity
            {
                SemesterId = semesterId,
                SubjectCode = subjectCode,
                RuleCode = ruleCode,
                DependencyACode = dependencyA,
                DependencyBCode = dependencyB
            };

            settings.Add(setting);
        }

        return settings;
    }
}