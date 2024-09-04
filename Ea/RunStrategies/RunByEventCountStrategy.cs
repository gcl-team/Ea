namespace Ea.RunStrategies;

public class RunByEventCountStrategy(int eventCount) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        for (var i = 0; i < eventCount; i++)
        {
            if (!simulator.ExecuteHeadEvent())
                return false;
        }
        return true;
    }
}