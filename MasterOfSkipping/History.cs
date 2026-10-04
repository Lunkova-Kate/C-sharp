namespace MasterOfSkipping;

public class History : IReadOnlyStudentHistory
{  
    private readonly List<DayHistory> _dayHistories  = new List<DayHistory>();

    public void RecordVisit(int day, Subject subject, bool wasAsked)
    {
        if (day == _dayHistories.Count)
        {
            _dayHistories.Add(new DayHistory());
           
        }
          _dayHistories[day].RecordVisitAtThisDay(subject, wasAsked);
    }

    public void RecordSkip(int day, Subject subject)
    {
        if (day == _dayHistories.Count)
        {
            _dayHistories.Add(new DayHistory());
            
        }
        _dayHistories[day].RecordSkipAtThisDay(subject);
    }

    public bool Attended(int day, Subject subject)
    {
        return _dayHistories[day].AttendedAtThisDay(subject);
    }

    public bool? WasAsked(int day, Subject subject)
    {
        return _dayHistories[day].WasAskedAtThisDay(subject);

    }
}