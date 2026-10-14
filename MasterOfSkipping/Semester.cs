
namespace MasterOfSkipping;

public class Semester
{
    private int _enjoyment;
    private int _daysElapsed = 0;
    private bool[] _yesterdayAsked = new bool[Enum.GetValues<Subject>().Length];
    private History _history;
    private readonly Professor[] _professors;
    private readonly ISkipStrategy _strategy;
    private const int SmallPie = 1;
    public int DaysElapsed => _daysElapsed;
    public int TotalEnjoyment => _enjoyment;
    public double AverageEnjoyment => _daysElapsed == 0 ? 0 : (double)_enjoyment / _daysElapsed;

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
        for (int i = 0; i < _yesterdayAsked.Length; ++i)
        {
            _yesterdayAsked[i] = false;
        }
        _history = new History();
    }

    public int Run(out int daysElapsed, out bool expelled)
    {
        while (_daysElapsed < SimulationSettings.DaysInSemester)
        {
            bool survived = ProcessDay();
            if (!survived)
            {
                daysElapsed = _daysElapsed;
                expelled = true;
                return _enjoyment;
            }
        }
        daysElapsed = SimulationSettings.DaysInSemester;
        expelled = false;
        return _enjoyment;
    }

    public bool ProcessDay()
    {

        bool[] attend = _strategy.DecideDay(_daysElapsed, _history);
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

            bool willAsk = _professors[i].WillAsk(_yesterdayAsked);
            todayAsked[i] = willAsk;
            if (attend[i])
            {
                _history.RecordVisit(_daysElapsed, (Subject)i, willAsk);
            }
            else
            {
                if (willAsk)
                {
                    _enjoyment = 0;
                    _history.RecordSkip(_daysElapsed, (Subject)i);
                    ++_daysElapsed;
                    return false;
                }
                else
                {
                    ++_enjoyment;
                    _history.RecordSkip(_daysElapsed, (Subject)i);

                }
            }

        }
        _enjoyment += SmallPie;
        _yesterdayAsked = todayAsked;
        ++_daysElapsed;
        return true;
    }

}