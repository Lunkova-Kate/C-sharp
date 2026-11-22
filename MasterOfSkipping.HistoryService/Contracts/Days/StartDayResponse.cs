namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record StartDayResponse(
    Guid SemesterId,
    int Day,
    int TotalEnjoyment,
    bool IsExpelled);
