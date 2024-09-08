namespace Ea.Standard;

public class RunByTimeStrategy(DateTime terminateDateTime) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        while (true)
        {
            if (!simulator.HasFutureEvents) return false;

            if (simulator.FutureEventList.First().ScheduledTime <= terminateDateTime)
            {
                simulator.ExecuteHeadEvent();
            }
            else
            {
                simulator.ClockTime = terminateDateTime;
                return true;
            }
        }
    }
}