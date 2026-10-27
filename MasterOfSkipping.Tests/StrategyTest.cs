using MasterOfSkipping;
using Xunit;

public class StrategyTests
{
    [Fact]
    public void DecideDay_EmptyHistory_AttendsAllLessons()
    {
        var strategy = new MyStrategy();
        var history = new History();

        bool[] actual = strategy.DecideDay(0, history);

        bool[] expected =
        [
            true,
            true,
            true,
            true,
            true,
            true
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DecideDay_NeverAskedForTenDays_SkipsAllLessons()
    {
        var strategy = new MyStrategy();

        History history = CreateSameHistory(
            days: 10,
            wasAsked: false);

        bool[] actual = strategy.DecideDay(10, history);

        bool[] expected =
        [
            false,
            false,
            false,
            false,
            false,
            false
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DecideDay_AlwaysAskedForTenDays_AttendsAllLessons()
    {
        var strategy = new MyStrategy();

        History history = CreateSameHistory(
            days: 10,
            wasAsked: true);

        bool[] actual = strategy.DecideDay(10, history);

        bool[] expected =
        [
            true,
            true,
            true,
            true,
            true,
            true
        ];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DecideDay_MixedHistory_DecidesSubjectsIndependently()
    {
        var strategy = new MyStrategy();

        History history = CreateMixedHistory();

        bool[] actual = strategy.DecideDay(10, history);

        bool[] expected =
        [
            false,
            true,  
            false, 
            false, 
            false, 
            false  
        ];

        Assert.Equal(expected, actual);
    }

    private static History CreateSameHistory(
        int days,
        bool wasAsked)
    {
        var history = new History();

        for (int day = 0; day < days; day++)
        {
            foreach (Subject subject in Enum.GetValues<Subject>())
            {
                history.RecordVisit(
                    day,
                    subject,
                    wasAsked);
            }
        }

        return history;
    }

    private static History CreateMixedHistory()
    {
        var history = new History();

        for (int day = 0; day < 10; day++)
        {
            foreach (Subject subject in Enum.GetValues<Subject>())
            {
                bool wasAsked = false;

                if (day == 0 &&
                    subject == Subject.LinearAlgebra)
                {
                    wasAsked = true;
                }

                if (day == 1 &&
                    subject == Subject.Calculus)
                {
                    wasAsked = true;
                }

                history.RecordVisit(
                    day,
                    subject,
                    wasAsked);
            }
        }

        return history;
    }
    
}