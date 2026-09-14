
namespace MasterOfSkipping;

public class Semester
{
    int enjoyment;
    private const int DaysInSemester = 100;
    bool[] yesterdayAsked = new bool[6];
    History history;
    Professor[] professors;
    ISkipStrategy strategy;
    private const int smallPie = 1;
    
    public Semester(Professor[] professors, ISkipStrategy strategy)
    {
        if (professors is null)
        {
            throw new ArgumentNullException(
                nameof(professors),
                "Professors cannot be null.");
        }

        int subjectCount = Enum.GetValues<Subject>().Length;

        if (professors.Length != subjectCount)
        {
            throw new ArgumentException(
                $"Expected {subjectCount} professors, but received {professors.Length}.",
                nameof(professors));
        }

        if (strategy is null)
        {
            throw new ArgumentNullException(
                nameof(strategy),
                "Strategy cannot be null.");
        }

        this.professors = professors;
        this.strategy = strategy;

        enjoyment = 0;
        for (int i = 0; i < yesterdayAsked.Length; ++i)
        {
            yesterdayAsked[i] = false;
        }
        history = new History();
    }

    public int Run(out int daysSurvived, out bool expelled)
    {
        for (int day = 0; day < 100; ++day)
        {
            if (!ProcessDay(day))
            {
                daysSurvived = day + 1;
                expelled = true;
                return 0;
            }
        }
        daysSurvived = 100;
        expelled = false;
        return enjoyment;
    }
    private bool ProcessDay(int dayNumber)
    {
        bool[] attend = strategy.DecideDay(dayNumber, history);
        if (attend is null)
        {
            throw new InvalidOperationException(
                $"strategy {strategy.Name} returned null array");
        }

        if (attend.Length != Enum.GetValues<Subject>().Length)
        {
            throw new InvalidOperationException(
                $"Strategy {strategy.Name} returned invalid array of length {attend.Length}");
        }


        bool[] todayAsked = new bool[6];

        for (int i = 0; i < todayAsked.Length; ++i)
        {

            bool willAsk = professors[i].WillAsk(yesterdayAsked);
            todayAsked[i] = willAsk;
            if (attend[i])
            {
                history.RecordVisit(dayNumber, (Subject)i, willAsk);
            }
            else
            {
                if (willAsk)
                {
                    enjoyment = 0;
                    history.RecordSkip(dayNumber, (Subject)i);
                    return false;
                }
                else
                {
                    ++enjoyment;
                    history.RecordSkip(dayNumber, (Subject)i);
                }
            }

        }
        enjoyment += smallPie;
        yesterdayAsked = todayAsked;
        return true;
    }

}