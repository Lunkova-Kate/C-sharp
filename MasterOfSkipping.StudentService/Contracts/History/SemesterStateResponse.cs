namespace MasterOfSkipping.StudentService.Contracts.History;

public sealed record SemesterStateResponse(
    Guid SemesterId,
    int Seed,
    int DaysCompleted,
    int TotalEnjoyment,
    bool IsExpelled);
