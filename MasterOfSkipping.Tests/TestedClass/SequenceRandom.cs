namespace MasterOfSkipping.Tests.TestedClass;

internal sealed class SequenceRandom : Random
{
    private readonly Queue<int> _values;

    public SequenceRandom(params int[] values)
    {
        _values = new Queue<int>(values);
    }

    public override int Next(int maxValue)
    {
        return NextValue(0, maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        return NextValue(minValue, maxValue);
    }

    private int NextValue(int minValue, int maxValue)
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("No configured random values remain.");
        }

        int value = _values.Dequeue();

        if (value < minValue || value >= maxValue)
        {
            throw new InvalidOperationException(
                $"Configured value {value} is outside [{minValue}, {maxValue}).");
        }

        return value;
    }
}
