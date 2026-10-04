namespace MasterOfSkipping;

public class Professor
{
    private readonly Subject _subjectA;
    private readonly Subject _subjectB;
    private readonly RuleType _rule;
    private readonly Random _random;


    public Subject Subject { get; }

    public Professor(Subject subject, RuleType rule, Random random, Subject a = default, Subject b = default)
    {
        Subject = subject;
        this._rule = rule;
        if (rule == RuleType.XorRule && a == b)
        {
            throw new ArgumentException("Subjects must be different for XorRule");
        }
        this._subjectA = a;
        this._subjectB = b;

        this._random = random;
    }

    public bool WillAsk(bool[] yesterdayAsked)
    {
        switch (_rule)
        {
            case RuleType.Random50:
                return Random50();
            case RuleType.YesterdayA:
                return YesterdayA(yesterdayAsked, _subjectA);
            case RuleType.XorRule:
                return XorRule(yesterdayAsked, _subjectA, _subjectB);
            default:
                throw new InvalidOperationException();
        }
    }

    private bool Random50()
    {
        return _random.Next(0, 2) == 1;
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
