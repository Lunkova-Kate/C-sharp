namespace MasterOfSkipping.Tests.TestedClass;

internal sealed class FixedRandom : Random
{
    private readonly int _value;

    public FixedRandom(int value)
    {
        _value = value;
    }

    public override int Next(int maxValue)
    {
        return _value;
    }

    public override int Next(int minValue, int maxValue)
    {
        return _value;
    }
}
