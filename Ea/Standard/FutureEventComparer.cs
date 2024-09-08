using Ea.Exceptions;

namespace Ea.Standard;

public class FutureEventComparer : IComparer<SimulationEventBase>
{
    public int Compare(SimulationEventBase? x, SimulationEventBase? y)
    {
        if (x is null || y is null)
            throw new SimulationException("One of the simulation event used for comparison is null");

        int compare = x.ScheduledTime.CompareTo(y.ScheduledTime);

        return compare == 0 ? x.Index.CompareTo(y.Index) : compare;
    }
}