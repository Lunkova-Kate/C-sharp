namespace MasterOfSkipping;
class DayHistory
{

 private static readonly int TotalSubjects = Enum.GetValues<Subject>().Length;
private  readonly bool[] _attended = new bool[TotalSubjects];
private readonly bool?[] _wasAsked = new bool?[TotalSubjects];

   public void RecordVisitAtThisDay( Subject subject, bool wasAsked)
    {
        _attended[(int)subject] = true;
        _wasAsked[(int)subject] = wasAsked;
    }

    public void RecordSkipAtThisDay( Subject subject)
    {
        _attended[(int)subject] = false;
        _wasAsked[(int)subject] = null;
    }

    public bool AttendedAtThisDay(Subject subject)
    {
        return _attended[(int)subject];
    }

    public bool? WasAskedAtThisDay(Subject subject)
    {
        return _wasAsked[(int)subject];
    }
}