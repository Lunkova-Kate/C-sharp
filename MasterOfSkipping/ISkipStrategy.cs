namespace MasterOfSkipping;

public interface ISkipStrategy
{
    string Name { get; }

    /// <summary>
    /// Решение на день: для каждого предмета — идти на пару (true) или прогулять (false).
    /// Вызывается один раз в начале каждого дня, до того как станут известны сегодняшние исходы.
    /// </summary>
    bool[] DecideDay(int day, IReadOnlyStudentHistory history);
}

public interface IReadOnlyStudentHistory

{
    /// <summary>Был ли студент на паре по предмету в указанный день.</summary>
    bool Attended(int day, Subject subject);
    /// <summary>
    /// Спросили ли по предмету в указанный день — известно только если Attended(day, subject) == true.
    /// Если студент прогулял и его не спросили — здесь null.
    /// </summary>

    bool? WasAsked(int day, Subject subject);

}