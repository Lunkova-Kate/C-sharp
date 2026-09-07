public class Professor
{
    Subject A;
    Subject B;
    RuleType Rule;

private static readonly Random _random = new Random();

    public Subject Subject { get;  }

    public Professor(Subject subject, RuleType rule, Subject a = default, Subject b = default)
{
    Subject = subject;
    Rule = rule;
    A = a;
    B = b;
}

public bool WillAsk(bool[] yesterdayAsked)
{
    switch (Rule)
    {
        case RuleType.Random50:
            return Random50();
        case RuleType.YesterdayA:
            return YesterdayA(yesterdayAsked, A);
        case RuleType.XorRule:
            return XorRule(yesterdayAsked, A, B);
        default:
            throw new InvalidOperationException();
    }
}

    private bool Random50()
    {
        return _random.Next(0, 2) == 1;
    }

    private bool YesterdayA(bool[] yesterdayAsked,Subject subjectA)
    {
        return yesterdayAsked[(int)A];
    }

    private bool XorRule(bool[] yesterdayAsked, Subject subjectA, Subject subjectB)
    {
        return yesterdayAsked[(int)A] ^ yesterdayAsked[(int)B];
    }
}
