namespace Ea.RunStrategies;

public class RunBySpeedStrategy(double speed) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        if (simulator.RealTimeForLastRun == null)
        {
            simulator.RealTimeForLastRun = DateTime.Now;
            return true;
        }

        var elapsedRealTime = DateTime.Now - simulator.RealTimeForLastRun.Value;
        var targetTime = simulator.ClockTime.AddSeconds(elapsedRealTime.TotalSeconds * speed);

        var result = new RunByTimeStrategy(targetTime).Run(simulator);
        simulator.RealTimeForLastRun = DateTime.Now;

        return result;
    }
}