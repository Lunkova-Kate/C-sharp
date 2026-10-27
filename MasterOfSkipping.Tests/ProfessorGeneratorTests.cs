using MasterOfSkipping;
using MasterOfSkipping.Tests.TestedClass;

public class ProfessorGeneratorTests
{
    int countSubject = Enum.GetValues<Subject>().Length;
    [Fact]
    public void Generate_ReturnsOneProfessorForSubjectInEnum()
    {
        Professor[] professors = ProfessorGenerator.Generate(new FixedRandom(0));

        Subject[] professorSubjects = Enum.GetValues<Subject>();
        Subject[] actualSubjects = professors.Select(professor => professor.Subject).ToArray();

        Assert.Equal(professorSubjects, actualSubjects);
    }

    [Fact]
    public void Generate_Random50Rule_ProfessorsCanAsk()
    {
        int[] values = [
            0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1
        ];

        var random = new SequenceRandom(values);

        Professor[] professors =
            ProfessorGenerator.Generate(random);

        bool[] yesterday = new bool[6];

        Assert.All(professors,
       professor => Assert.True(professor.WillAsk(yesterday)));
    }

    [Fact]
    public void Generate_YesterdayARule_UsesSubjectA()
    {
        var random = new FixedRandom(1);
        Professor[] professors = ProfessorGenerator.Generate(random);
        bool[] notAskedYesterday = new bool[6];
        Assert.All(professors,
        professor => Assert.False(professor.WillAsk(notAskedYesterday)));


        bool[] askedYesterday = new bool[6];
        askedYesterday[(int)Subject.LinearAlgebra] = true;
        Assert.All(professors,
        professor => Assert.True(professor.WillAsk(askedYesterday)));

    }

    [Fact]
    public void Generate_XorRule_UsesTwoDifferentSubjects()
    {
        int[] randomValues =
        [
            
        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra,

        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra,

        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra,

        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra,

        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra,

        (int)RuleType.XorRule,
        (int)Subject.Calculus,
        (int)Subject.Calculus,
        (int)Subject.LinearAlgebra
        ];

        var random = new SequenceRandom(randomValues);

        Professor[] professors =
            ProfessorGenerator.Generate(random);


        bool[] onlyCalculusAsked = new bool[6];
        onlyCalculusAsked[(int)Subject.Calculus] = true;

        Assert.All(
            professors,
            professor =>
                Assert.True(professor.WillAsk(onlyCalculusAsked)));

        bool[] bothSubjectsAsked = new bool[6];
        bothSubjectsAsked[(int)Subject.Calculus] = true;
        bothSubjectsAsked[(int)Subject.LinearAlgebra] = true;

        Assert.All(
            professors,
            professor =>
                Assert.False(professor.WillAsk(bothSubjectsAsked)));
    }
}
