using Ea.Standard;
using Ea.StaticConfigs;

namespace Ea.Sandboxes;

/// <summary>
/// Represents a load in the simulation, which tracks events and their associated timestamps.
/// </summary>
public class Load : SimulationSandboxBase<LoadStaticConfig>
{
    /// <summary>
    /// Gets the list of timestamps associated with events.
    /// </summary>
    public List<Tuple<DateTime, SimulationEventBase>> TimeStamps { get; private set; }

    /// <summary>
    /// Gets the total time span between the first and last events.
    /// </summary>
    public TimeSpan TotalTimeSpan =>
        TimeStamps.Max(t => t.Item1) - TimeStamps.Min(t => t.Item1);

    /// <summary>
    /// Initializes a new instance of the <see cref="Load"/> class.
    /// </summary>
    /// <param name="seed">The seed for randomization.</param>
    /// <param name="tag">An optional tag for the load.</param>
    public Load(int seed, string tag) : base(new LoadStaticConfig(), seed, "Load", tag)
    {
        TimeStamps = new List<Tuple<DateTime, SimulationEventBase>>();
    }

    /// <summary>
    /// Retrieves the first timestamp that matches the specified condition.
    /// </summary>
    /// <param name="check">A function to evaluate each event. If null, returns the first timestamp.</param>
    /// <returns>The first matching timestamp, or null if none found.</returns>
    public DateTime? GetFirstTimeStamp(Func<SimulationEventBase, bool> check = null)
    {
        foreach (var timeStamp in TimeStamps)
        {
            if (check == null || check(timeStamp.Item2))
                return timeStamp.Item1;
        }
        return null;
    }

    /// <summary>
    /// Retrieves the last timestamp that matches the specified condition.
    /// </summary>
    /// <param name="check">A function to evaluate each event. If null, returns the last timestamp.</param>
    /// <returns>The last matching timestamp, or null if none found.</returns>
    public DateTime? GetLastTimeStamp(Func<SimulationEventBase, bool> check)
    {
        for (int i = TimeStamps.Count - 1; i >= 0; i--)
        {
            if (check == null || check(TimeStamps[i].Item2))
                return TimeStamps[i].Item1;
        }
        return null;
    }

    /// <summary>
    /// Logs an event and returns the logged event.
    /// </summary>
    /// <param name="simulationEvent">The event to be logged.</param>
    /// <returns>The logged event.</returns>
    public SimulationEventBase Log(SimulationEventBase simulationEvent)
    {
        return new LogEvent
        {
            AssociatedSandbox = this,
            ToLogEvent = simulationEvent
        };
    }

    /// <summary>
    /// Method to handle actions when the simulation is warmed up.
    /// </summary>
    /// <param name="clockTime">The current clock time.</param>
    public override void WarmedUp(DateTime clockTime) { }

    private abstract class InternalEvent : SimulationEventBase<Load, LoadStaticConfig> { }

    private sealed class LogEvent : InternalEvent
    {
        internal SimulationEventBase ToLogEvent { private get; set; }

        public override void Invoke()
        {
            AssociatedSandbox.TimeStamps.Add(new Tuple<DateTime, SimulationEventBase>(ClockTime, ToLogEvent));
        }
    }
}