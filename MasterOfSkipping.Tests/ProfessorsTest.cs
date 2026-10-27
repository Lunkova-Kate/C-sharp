using Xunit;
using MasterOfSkipping;
using MasterOfSkipping.Tests.TestedClass;

public class ProfessorsTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void WillAsk_YesterdayARule_ReturnsStateOfSubjectA(bool input, bool expected)
    {
        var professor = new Professor(Subject.English, RuleType.YesterdayA, new Random(42), Subject.Calculus);
        var yesterday = new bool[6];
        yesterday[(int)Subject.Calculus] = input;
        bool actual = professor.WillAsk(yesterday);
        Assert.Equal(expected, actual);

    }
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public void WillAsk_XorRule_ReturnStateOfSubject(bool askedA, bool askedB, bool expected)
    {
        var professor = new Professor(Subject.English, RuleType.XorRule, new Random(42), Subject.Calculus, Subject.Physics);
        var yesterday = new bool[6];
        yesterday[(int)Subject.Calculus] = askedA;
        yesterday[(int)Subject.Physics] = askedB;

        bool actual = professor.WillAsk(yesterday);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void WillAsk_Random50Rule_ReturnsValueFromRandom(int randomValue, bool expected)
    {
        var professor = new Professor(
            Subject.English,
            RuleType.Random50,
            new FixedRandom(randomValue));

        bool actual = professor.WillAsk(new bool[6]);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Constructor_XorRuleWithSameSubjects_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Professor(
                Subject.English,
                RuleType.XorRule,
                new Random(42),
                Subject.Calculus,
                Subject.Calculus));
    }

    [Fact]
    public void WillAsk_UnknownRuleType_ThrowsInvalidOperationException()
    {
        var unknownRule = (RuleType)999;
        var professor = new Professor(
            Subject.English,
            unknownRule,
            new Random(42));

        Assert.Throws<InvalidOperationException>(() =>
            professor.WillAsk(new bool[6]));
    }

}
