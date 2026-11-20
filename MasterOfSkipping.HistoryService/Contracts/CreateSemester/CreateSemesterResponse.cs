namespace MasterOfSkipping.HistoryService.Contracts;


public sealed record CreateSemesterResponse(Guid SemesterId, int? Seed);