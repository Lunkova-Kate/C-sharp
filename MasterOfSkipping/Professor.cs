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

 

// У каждого преподавателя — одно из трёх правил:

// Спросить случайно с вероятностью 50%.
// Спросить, если вчера спросили по предмету A иначе не спрашивать
// Спросить, если вчера спросили по предмету A и не спросили по предмету Б либо не спросили по предмету A и спросили по предмету\
//  Б, иначе не спрашивать. A & -Б || -A & Б
// (Предметы A и Б для правил 2 и 3 у каждого преподавателя свои, выбираются случайно в начала семестра и могут \
// совпадать с его собственным предметом, \
// но не могут совпадать друг с другом. В первый день семестра считается, что вчера никого не спрашивали. А!=Б)