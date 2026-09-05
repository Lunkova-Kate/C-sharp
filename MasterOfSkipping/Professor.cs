public class Professor
{

private static readonly Random _random = new Random();

    public Subject Subject { get;  }

    public Professor(string name, Subject subject)
    {
        Subject = subject;
    }


    bool Random50()
    {
        return _random.Next(0, 2) == 1;
    }

    bool YesterdayA(Subject subjectA)
    {

    }

    bool XorRule(Subject subjectA, Subject subjectB)
    {

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