namespace MasterOfSkipping;

public sealed class MyStrategy : ISkipStrategy
{
    private const int SubjectCount = 6;
    private const int HypothesisCount = 21;
    private const int MinimumObservationDays = 10;
    private const int AllHypothesesMask = (1 << HypothesisCount) - 1;
    private const int AskedStateCount = 1 << SubjectCount;

    private static readonly byte[] SubjectsUsedByHypothesis = BuildHypotheses();

    private static readonly int[] AskingHypothesesByState =
        BuildAskingHypothesesByState();

    private readonly int[] _possibleHypotheses = new int[SubjectCount];

    private readonly bool[] _decision = new bool[SubjectCount];

    private IReadOnlyStudentHistory? _currentHistory;
    private int _processedDays;
    private byte _askedYesterday;

    public MyStrategy()
    {
        Reset(null);
    }

    public string Name => "My Strategy";

    public bool[] DecideDay(int currentDay, IReadOnlyStudentHistory history)
    {
        ArgumentNullException.ThrowIfNull(history);

        if (currentDay < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentDay),
                "Day cannot be negative.");
        }

        bool differentHistory =
            _currentHistory is not null &&
            !ReferenceEquals(_currentHistory, history);

        if (currentDay == 0 || differentHistory || currentDay < _processedDays)
        {
            Reset(history);
        }
        else if (_currentHistory is null)
        {
            _currentHistory = history;
        }

        ProcessNewDays(currentDay, history);

        if (currentDay < MinimumObservationDays)
        {
            Array.Fill(_decision, true);
            return _decision;
        }

        int askingToday = AskingHypothesesByState[_askedYesterday];

        for (int subject = 0; subject < SubjectCount; subject++)
        {
            int possible = _possibleHypotheses[subject];

            bool allPossibleRulesPredictNoQuestion =
                possible != 0 &&
                (possible & askingToday) == 0;

            _decision[subject] = !allPossibleRulesPredictNoQuestion;
        }

        return _decision;
    }

    private void ProcessNewDays(
        int currentDay,
        IReadOnlyStudentHistory history)
    {
        while (_processedDays < currentDay)
        {
            byte askedToday = ReadAskedMask(_processedDays, history);
            int askingHypotheses =
                AskingHypothesesByState[_askedYesterday];
            int notAskingHypotheses =
                AllHypothesesMask ^ askingHypotheses;

            for (int subject = 0; subject < SubjectCount; subject++)
            {
                bool wasAsked =
                    (askedToday & (1 << subject)) != 0;

                _possibleHypotheses[subject] &= wasAsked
                    ? askingHypotheses
                    : notAskingHypotheses;
            }

            _askedYesterday = askedToday;
            _processedDays++;
        }
    }

    private static byte ReadAskedMask(
        int day,
        IReadOnlyStudentHistory history)
    {
        byte asked = 0;

        for (int subjectIndex = 0;
             subjectIndex < SubjectCount;
             subjectIndex++)
        {
            var subject = (Subject)subjectIndex;

            if (history.Attended(day, subject) &&
                history.WasAsked(day, subject) == true)
            {
                asked |= (byte)(1 << subjectIndex);
            }
        }

        return asked;
    }

    private void Reset(IReadOnlyStudentHistory? history)
    {
        _currentHistory = history;
        _processedDays = 0;
        _askedYesterday = 0;
        Array.Fill(_possibleHypotheses, AllHypothesesMask);
        Array.Fill(_decision, true);
    }

    private static bool Predict(byte askedYesterday, byte hypothesis)
    {
        int matchingAskedSubjects = askedYesterday & hypothesis;

        return matchingAskedSubjects != 0 &&
               (matchingAskedSubjects & (matchingAskedSubjects - 1)) == 0;
    }

    private static byte[] BuildHypotheses()
    {
        var hypotheses = new byte[HypothesisCount];
        int index = 0;

        for (int subject = 0; subject < SubjectCount; subject++)
        {
            hypotheses[index++] = (byte)(1 << subject);
        }

        for (int first = 0; first < SubjectCount; first++)
        {
            for (int second = first + 1;
                 second < SubjectCount;
                 second++)
            {
                hypotheses[index++] =
                    (byte)((1 << first) | (1 << second));
            }
        }

        return hypotheses;
    }

    private static int[] BuildAskingHypothesesByState()
    {
        var result = new int[AskedStateCount];

        for (int askedState = 0;
             askedState < AskedStateCount;
             askedState++)
        {
            int askingHypotheses = 0;

            for (int hypothesisIndex = 0;
                 hypothesisIndex < HypothesisCount;
                 hypothesisIndex++)
            {
                if (Predict(
                        (byte)askedState,
                        SubjectsUsedByHypothesis[hypothesisIndex]))
                {
                    askingHypotheses |= 1 << hypothesisIndex;
                }
            }

            result[askedState] = askingHypotheses;
        }

        return result;
    }
}
