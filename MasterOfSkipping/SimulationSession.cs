namespace MasterOfSkipping;

public class SimulationSession
{
    public Guid SemesterId { get; }
    public Semester Semester { get; }

    public SimulationSession(Guid semesterId, Semester semester)
    {
        SemesterId = semesterId;
        Semester = semester;
    }
}