namespace MasterOfSkipping;

public class DayResult
{
    public  int DayNumber { get; }
    public  int TotalEnjoyment { get; }
    public bool IsExpelled { get; }
    public  bool[] Attendance { get; }
    public  bool[] Asked { get; }
    public int ProcessedLessonCount { get; }

    public DayResult(int dayNumber, int totalEnjoyment, bool isExpelled, bool[] attendance, bool[] asked, int processedLessonCount)
    {
        DayNumber = dayNumber;
        TotalEnjoyment = totalEnjoyment;
        IsExpelled = isExpelled;
        Attendance = attendance;
        Asked = asked;
        ProcessedLessonCount = processedLessonCount;
    }
}