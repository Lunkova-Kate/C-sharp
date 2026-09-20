namespace MasterOfSkipping;

public class Professor
{
    private readonly Subject a;
    private readonly Subject b;
    private readonly RuleType rule;
    private readonly Random random;


    public Subject Subject { get; }

    public Professor(Subject subject, RuleType rule, Random random, Subject a = default, Subject b = default)
    {
        Subject = subject;
        this.rule = rule;
        if (rule == RuleType.XorRule && a == b)
        {
            throw new ArgumentException("Subjects must be different for XorRule");
        }
        this.a = a;
        this.b = b;

        this.random = random;
    }

    public bool WillAsk(bool[] yesterdayAsked)
    {
        switch (rule)
        {
            case RuleType.Random50:
                return Random50();
            case RuleType.YesterdayA:
                return YesterdayA(yesterdayAsked, a);
            case RuleType.XorRule:
                return XorRule(yesterdayAsked, a, b);
            default:
                throw new InvalidOperationException();
        }
    }

    private bool Random50()
    {
        return random.Next(0, 2) == 1;
    }

    private bool YesterdayA(bool[] yesterdayAsked, Subject subjectA)
    {
        return yesterdayAsked[(int)subjectA];
    }

    private bool XorRule(bool[] yesterdayAsked, Subject subjectA, Subject subjectB)
    {
        return yesterdayAsked[(int)subjectA] ^ yesterdayAsked[(int)subjectB];
    }
}
