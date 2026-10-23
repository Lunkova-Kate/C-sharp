using Xunit;
using MasterOfSkipping;
using MasterOfSkipping.Tests.TestedClass;
public class SemestrTests
{

    [Theory]
    [InlineData(true, true, true,1)]
    [InlineData(true, false, true, 1)]
    [InlineData(false, false, true, 2)]
    [InlineData(false, true,false, 0)]
    public void ProcessDay_HandlesAttendanceAndQuestion(
        bool attendLessons,
        bool professorAsks,
        bool survive,
        int enjoyment
    )
    {
        Professor[] professors = CreateProfessors(professorAsks);
            bool[] decisions =
        [
            true,
            true,
            true,
            true,
            true,
            true
        ];

        decisions[0] = attendLessons;
        var strategy = new FixedStrategy(decisions);
        var semester = new Semester(professors, strategy);
        bool actualSurvived = semester.ProcessDay();
        Assert.Equal(survive, actualSurvived);
        Assert.Equal(enjoyment, semester.TotalEnjoyment);
        Assert.Equal(1, semester.DaysElapsed);

    }

    private static Professor[] CreateProfessors(
        bool firstProfessorAsks)
    {
        int subjectCount = Enum.GetValues<Subject>().Length;

        var professors = new Professor[subjectCount];

        for (int i = 0; i < subjectCount; i++)
        {
            int randomValue = 0;

            if (i == 0 && firstProfessorAsks)
            {
                randomValue = 1;
            }

            professors[i] = new Professor(
                (Subject)i,
                RuleType.Random50,
                new FixedRandom(randomValue));
        }

        return professors;
    }
}