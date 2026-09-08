namespace MasterOfSkipping;
public class AlwaysAttendStrategy : ISkipStrategy
{
    public string Name => "Always attend";

    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
    {
        return new bool[] { true, true, true, true, true, true };
    }
}