using System;

public class History : IReadOnlyStudentHistory
{
    private const int TotalDays = 100;
    private const int TotalSubjects = 6;// maybe use Enum.GetValue().lenght

    private bool[,] attended = new bool[TotalDays, TotalSubjects];
    private bool?[,] wasAsked = new bool?[TotalDays, TotalSubjects];

    public void RecordVisit(int day, Subject subject, bool wasAsked)
    {
        attended[day, (int)subject] = true;
        this.wasAsked[day, (int)subject] = wasAsked;
    }

    public void RecordSkip(int day, Subject subject)
    {
        attended[day, (int)subject] = false;
        this.wasAsked[day, (int)subject] = null;
    }

    public bool Attended(int day, Subject subject)
    {
        return attended[day, (int)subject];
    }

    public bool? WasAsked(int day, Subject subject)
    {
        return wasAsked[day, (int)subject];
    }
}