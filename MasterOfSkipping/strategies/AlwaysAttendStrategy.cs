namespace MasterOfSkipping;

public class AlwaysAttendStrategy : ISkipStrategy
{
    
    public string Name => "Always attend";

    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
    {
        
        return day < 99
            ? new bool[] { true, true, true, true, true, true }   
            : new bool[] { false, false, false, false, false, false }; 
    }
     
    
}