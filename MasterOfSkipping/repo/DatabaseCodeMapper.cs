namespace MasterOfSkipping;

public static class DatabaseCodeMapper
{
    public static string SubjectToCode(Subject subject)
    {
        switch (subject)
        {
            case Subject.Calculus:
                return "CALCULUS";
            case Subject.LinearAlgebra:
                return "LINEAR_ALGEBRA";
            case Subject.DiscreteMath:
                return "DISCRETE_MATH";
            case Subject.Programming:
                return "PROGRAMMING";
            case Subject.Physics:
                return "PHYSICS";
            case Subject.English:
                return "ENGLISH";
            default:
                throw new ArgumentOutOfRangeException(nameof(subject), subject, null);
        }
    }

    public static string RuleToCode(RuleType rule)
    {
        switch (rule)
        {
            case RuleType.Random50:
                return "RANDOM_50";
            case RuleType.YesterdayA:
                return "YESTERDAY_A";
            case RuleType.XorRule:
                return "XOR";
            default:
                throw new ArgumentOutOfRangeException(nameof(rule), rule, null);
        }
    }
}