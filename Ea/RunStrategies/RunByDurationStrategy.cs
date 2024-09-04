namespace Ea.RunStrategies;

public class RunByDurationStrategy(TimeSpan duration) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        var targetTime = simulator.ClockTime.Add(duration);
        return new RunByTimeStrategy(targetTime).Run(simulator);
    }
}