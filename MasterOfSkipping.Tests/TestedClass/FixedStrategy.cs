using MasterOfSkipping;

namespace MasterOfSkipping.Tests.TestedClass;

internal sealed class FixedStrategy : ISkipStrategy
{
    private readonly bool[] _decisions;

    public string Name => "Fixed test strategy";

    public int CallCount { get; private set; }

    public FixedStrategy(params bool[] decisions)
    {
        _decisions = decisions;
    }

    public bool[] DecideDay(
        int day,
        IReadOnlyStudentHistory history)
    {
        CallCount++;

        return (bool[])_decisions.Clone();
    }
}