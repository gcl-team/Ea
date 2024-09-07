namespace Ea.Standard;

public class FutureEventComparer : IComparer<SimulationEventBase>
{
    public int Compare(SimulationEventBase x, SimulationEventBase y)
    {
        int compare = x.ScheduledTime.CompareTo(y.ScheduledTime);
        if (compare == 0) return x.Index.CompareTo(y.Index);
        return compare;
    }
}