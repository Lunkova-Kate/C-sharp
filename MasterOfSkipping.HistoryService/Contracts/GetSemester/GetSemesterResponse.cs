namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record GetSemesterResponse(
    Guid SemesterId,
    int Seed,
    int DaysCompleted,
    int TotalEnjoyment,
    bool IsExpelled);