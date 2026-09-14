namespace MasterOfSkipping;

public class History : IReadOnlyStudentHistory
{
    private const int TotalDays = 100;
    private static readonly int TotalSubjects = Enum.GetValues<Subject>().Length;
    private bool[,] _attended = new bool[TotalDays, TotalSubjects];
    private bool?[,] _wasAsked = new bool?[TotalDays, TotalSubjects];

    public void RecordVisit(int day, Subject subject, bool wasAsked)
    {
        _attended[day, (int)subject] = true;
        _wasAsked[day, (int)subject] = wasAsked;
    }

    public void RecordSkip(int day, Subject subject)
    {
        _attended[day, (int)subject] = false;
        _wasAsked[day, (int)subject] = null;
    }

    public bool Attended(int day, Subject subject)
    {
        return _attended[day, (int)subject];
    }

    public bool? WasAsked(int day, Subject subject)
    {
        return _wasAsked[day, (int)subject];
    }
}