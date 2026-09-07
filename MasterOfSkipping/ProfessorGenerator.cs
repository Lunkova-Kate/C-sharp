using System;

namespace MasterOfSkipping
{
    public static class ProfessorGenerator
    {
        public static Professor[] Generate()
        {
            var professors = new Professor[6];
            var random = new Random();

            for (int i = 0; i < professors.Length; i++)
            {
                Subject subject = (Subject)i;

                RuleType ruleType = (RuleType)random.Next(3);

                switch (ruleType)
                {
                    case RuleType.Random50:
                        professors[i] = new Professor(subject, RuleType.Random50);
                        break;

                    case RuleType.YesterdayA:
                        Subject a = (Subject)random.Next(6);
                        professors[i] = new Professor(subject, RuleType.YesterdayA, a);
                        break;

                    case RuleType.XorRule:
                        Subject first = (Subject)random.Next(6);
                        Subject second;
                        do
                        {
                            second = (Subject)random.Next(6);
                        } while (first == second);
                        professors[i] = new Professor(subject, RuleType.XorRule, first, second);
                        break;
                }
            }

            return professors;
        }
    }
}