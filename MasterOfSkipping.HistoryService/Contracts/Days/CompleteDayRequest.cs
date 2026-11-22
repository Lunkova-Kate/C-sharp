namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record CompleteDayRequest(
    int TotalEnjoyment,
    bool IsExpelled);
