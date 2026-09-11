using System;

namespace MasterOfSkipping;

public static class ProfessorGenerator
{
    public static Professor[] Generate(Random random)
    {

        var professors = new Professor[6];

        for (int i = 0; i < professors.Length; i++)
        {
            Subject subject = (Subject)i;
            RuleType ruleType = (RuleType)random.Next(3);

            switch (ruleType)
            {
                case RuleType.Random50:
                    professors[i] = new Professor(
                        subject,
                        ruleType,
                        random);
                    break;

                case RuleType.YesterdayA:
                    Subject a = (Subject)random.Next(6);

                    professors[i] = new Professor(
                        subject,
                        ruleType,
                        random,
                        a);
                    break;

                case RuleType.XorRule:
                    Subject first = (Subject)random.Next(6);
                    Subject second;

                    do
                    {
                        second = (Subject)random.Next(6);
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