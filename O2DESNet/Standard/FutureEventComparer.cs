namespace O2DESNet.Standard;

public class FutureEventComparer : IComparer<SimulationEvent>
{
    public int Compare(SimulationEvent x, SimulationEvent y)
    {
        int compare = x.ScheduledTime.CompareTo(y.ScheduledTime);
        if (compare == 0) return x.Index.CompareTo(y.Index);
        return compare;
    }
}