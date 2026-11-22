namespace MasterOfSkipping.StudentService.Contracts.Professor;

public sealed record AskProfessorRequest(Guid SemesterId, int Day, bool Attended);
