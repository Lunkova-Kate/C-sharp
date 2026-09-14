
namespace MasterOfSkipping;

public class Semester
{
    private int _enjoyment;
    bool[] yesterdayAsked = new bool[Enum.GetValues<Subject>().Length];
    private History _history;
    private readonly Professor[] _professors;
    private readonly ISkipStrategy _strategy;
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

        this._professors = professors;
        this._strategy = strategy;

        _enjoyment = 0;
        for (int i = 0; i < yesterdayAsked.Length; ++i)
        {
            yesterdayAsked[i] = false;
        }
        _history = new History();
    }

    public int Run(out int daysElapsed, out bool expelled)
    {
        daysElapsed = 0;
        for (int day = 0; day < SimulationSettings.DaysInSemester; ++day)
        {
            if (!ProcessDay(day))
            {
                daysElapsed = day + 1;
                expelled = true;
                return 0;
            }
        }
        daysElapsed = SimulationSettings.DaysInSemester;
        expelled = false;
        return _enjoyment;
    }
    private bool ProcessDay(int dayNumber)
    {
        bool[] attend = _strategy.DecideDay(dayNumber, _history);
        if (attend is null)
        {
            throw new InvalidOperationException(
                $"strategy {_strategy.Name} returned null array");
        }

        if (attend.Length != Enum.GetValues<Subject>().Length)
        {
            throw new InvalidOperationException(
                $"Strategy {_strategy.Name} returned invalid array of length {attend.Length}");
        }


        bool[] todayAsked = new bool[Enum.GetValues<Subject>().Length];

        for (int i = 0; i < todayAsked.Length; ++i)
        {

            bool willAsk = _professors[i].WillAsk(yesterdayAsked);
            todayAsked[i] = willAsk;
            if (attend[i])
            {
                _history.RecordVisit(dayNumber, (Subject)i, willAsk);
            }
            else
            {
                if (willAsk)
                {
                    _enjoyment = 0;
                    _history.RecordSkip(dayNumber, (Subject)i);
                    return false;
                }
                else
                {
                    ++_enjoyment;
                    _history.RecordSkip(dayNumber, (Subject)i);
                }
            }

        }
        _enjoyment += smallPie;
        yesterdayAsked = todayAsked;
        return true;
    }

}