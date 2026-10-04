namespace MasterOfSkipping;

public static class ProfessorGenerator
{
    public static Professor[] Generate(Random random)
    {
        var subjectCount = Enum.GetValues<Subject>().Length;
        var ruleTypes = Enum.GetValues<RuleType>();
        var professors = new Professor[subjectCount];

        for (int i = 0; i < professors.Length; ++i)
        {
            var subject = (Subject)i;
            var ruleType = ruleTypes[random.Next(ruleTypes.Length)];

            switch (ruleType)
            {
                case RuleType.Random50:
                    professors[i] = new Professor(
                        subject,
                        ruleType,
                        random);
                    break;

                case RuleType.YesterdayA:
                    Subject a = (Subject)random.Next(subjectCount);

                    professors[i] = new Professor(
                        subject,
                        ruleType,
                        random,
                        a);
                    break;

                case RuleType.XorRule:
                    Subject first = (Subject)random.Next(subjectCount);
                    Subject second;

                    do
                    {
                        second = (Subject)random.Next(subjectCount);
                    }
                    while (first == second);

                    professors[i] = new Professor(
                        subject,
                        ruleType,
                        random,
                        first,
                        second);
                    break;
            }
        }

        return professors;
    }
}
