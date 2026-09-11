
namespace MasterOfSkipping;

public class Semestr
{
    int enjoyment;
    bool[] yesterdayAsked = new bool[6];
    History history;
    Professor[] professors;
    ISkipStrategy strategy;
    private const int smallPie = 1;

    public Semestr(Professor[] professors, ISkipStrategy strategy)
    {
        if (professors is null)
        {
            throw new ArgumentNullException(
                nameof(professors),
                "Professors cannot be null");
        }
        if (strategy is null)
        {
            throw new ArgumentNullException(
                nameof(strategy),
                "Strategy cannot be null");
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

    public int Run()
    {
        for (int day = 0; day < 100; ++day)
        {
            if (!ProcessDay(day))
                return 0;
        }
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

        int subjectCount = Enum.GetValues<Subject>().Length;

        if (professors.Length != subjectCount)
        {
            throw new ArgumentException(
                $"Expected {subjectCount} professors, but received {professors.Length}.",
                nameof(professors));
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