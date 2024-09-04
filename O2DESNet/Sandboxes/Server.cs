using O2DESNet.Exceptions;
using O2DESNet.Standard;
using O2DESNet.StaticConfigs;

namespace O2DESNet.Sandboxes;

/// <summary>
/// Represents a Server in a discrete event simulation that processes loads.
/// </summary>
/// <typeparam name="TLoad">The type of load that the Server processes.</typeparam>
public class Server<TLoad> : SimulationSandbox<ServerStaticConfig<TLoad>>
{
    /// <summary>
    /// The set of loads currently being processed by the Server.
    /// </summary>
    public HashSet<TLoad> Serving { get; private set; } = [];

    /// <summary>
    /// The set of loads that have been processed by the Server.
    /// </summary>
    public HashSet<TLoad> Served { get; private set; } = [];

    /// <summary>
    /// Gets the number of available slots for new loads.
    /// </summary>
    public int Vacancy => SimulationStaticConfig.Capacity - Occupancy;

    /// <summary>
    /// Gets the total number of loads, both currently being served and already served.
    /// </summary>
    public int Occupancy => Serving.Count + Served.Count;

    /// <summary>
    /// Tracks Server utilization over time.
    /// </summary>
    public TimeBasedMetric UtilizationCounter { get; private set; } = new();

    /// <summary>
    /// Tracks Server occupation over time.
    /// </summary>
    public TimeBasedMetric OccupationCounter { get; private set; } = new();

    /// <summary>
    /// Gets the number of loads processed by the Server, including those that have not departed.
    /// </summary>
    public int NCompleted => (int)UtilizationCounter.TotalDecrementCount;

    /// <summary>
    /// Gets the Server utilization rate as a percentage of its capacity.
    /// </summary>
    public double Utilization => UtilizationCounter.AverageCount / SimulationStaticConfig.Capacity;

    /// <summary>
    /// Gets the Server occupation rate as a percentage of its capacity.
    /// </summary>
    public double Occupation => OccupationCounter.AverageCount / SimulationStaticConfig.Capacity;

    /// <summary>
    /// Stores the start time of each load.
    /// </summary>
    public Dictionary<TLoad, DateTime> StartTimeRecords { get; private set; } = new();

    /// <summary>
    /// Stores the finish time of each load.
    /// </summary>
    public Dictionary<TLoad, DateTime> FinishTimeRecords { get; private set; } = new();

    /// <summary>
    /// Indicates whether the Server is ready for loads to depart.
    /// </summary>
    public bool IsReadyToDepart { get; protected set; } = true;

    /// <summary>
    /// Adds a load to the Server set.
    /// </summary>
    /// <param name="load">The load to be added.</param>
    protected virtual void PushIn(TLoad load) { Serving.Add(load); }

    /// <summary>
    /// Checks if the Server is ready for any loads to depart.
    /// </summary>
    /// <returns>True if ready to depart; otherwise, false.</returns>
    protected virtual bool CheckIsReadyToDepart()
    {
        return IsReadyToDepart && Served.Count > 0;
    }

    /// <summary>
    /// Retrieves the next load that is ready to depart.
    /// </summary>
    /// <returns>The load ready to depart, or default if none.</returns>
    protected virtual TLoad GetReadyToDepartLoad()
    {
        return Served.FirstOrDefault();
    }
    
    /// <summary>
    /// Base class for internal events in the Server class.
    /// </summary>
    private abstract class InternalEvent : SimulationEvent<Server<TLoad>, ServerStaticConfig<TLoad>> { }

    /// <summary>
    /// Represents an event where a load starts being processed.
    /// </summary>
    private class StartEvent : InternalEvent
    {
        internal TLoad Load { get; set; }

        public override void Invoke()
        {
            if (AssociatedSandbox.Vacancy < 1) 
                throw new SimulationException("Make sure the vacancy of the Server is not zero before execute Start event.");
            
            AssociatedSandbox.PushIn(Load);
            AssociatedSandbox.UtilizationCounter.ObserveChange(1, ClockTime);
            AssociatedSandbox.OccupationCounter.ObserveChange(1, ClockTime);
            AssociatedSandbox.StartTimeRecords.Add(Load, ClockTime);
            
            Schedule(new FinishEvent { Load = Load }, Config.ServiceTime(Load, DefaultRs));
            
            Execute(new StateChangeEvent());
        }

        public override string ToString() { return string.Format("{0}_StartEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Represents an event where a load finishes processing.
    /// </summary>
    private class FinishEvent : InternalEvent
    {
        internal TLoad Load { get; set; }

        public override void Invoke()
        {
            AssociatedSandbox.Serving.Remove(Load);
            AssociatedSandbox.Served.Add(Load);
            AssociatedSandbox.FinishTimeRecords.Add(Load, ClockTime);
            AssociatedSandbox.UtilizationCounter.ObserveChange(-1, ClockTime);
            Execute(new StateChangeEvent());
            if (AssociatedSandbox.CheckIsReadyToDepart()) Execute(new DepartEvent());
        }

        public override string ToString() { return string.Format("{0}_FinishEvent", AssociatedSandbox); }
    }
    
    /// <summary>
    /// Represents an event to update the Server IsReadyToDepart flag.
    /// </summary>
    private class UpdateIsReadyToDepartEvent : InternalEvent
    {
        internal bool IsReadyToDepart { get; set; }

        public override void Invoke()
        {
            AssociatedSandbox.IsReadyToDepart = IsReadyToDepart;
            if (AssociatedSandbox.CheckIsReadyToDepart()) Execute(new DepartEvent());
        }

        public override string ToString() { return string.Format("{0}_UpdateToDepartEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Represents an event to signal a state change in the Server.
    /// </summary>
    private class StateChangeEvent : InternalEvent
    {
        public override void Invoke() { Execute(AssociatedSandbox.OnStateChange.Select(e => e())); }

        public override string ToString() { return string.Format("{0}_StateChangeEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Represents an event where a load departs from the Server.
    /// </summary>
    private class DepartEvent : InternalEvent
    {
        public override void Invoke()
        {
            TLoad load = AssociatedSandbox.GetReadyToDepartLoad();
            AssociatedSandbox.Served.Remove(load);
            AssociatedSandbox.OccupationCounter.ObserveChange(-1, ClockTime);
            // in case the start/finish times are used in OnDepart events
            AssociatedSandbox.StartTimeRecords.Remove(load);
            AssociatedSandbox.FinishTimeRecords.Remove(load);
            foreach (var simulationEvent in AssociatedSandbox.OnDepart) Execute(simulationEvent(load));

            Execute(new StateChangeEvent());
            if (AssociatedSandbox.CheckIsReadyToDepart()) Execute(new DepartEvent());
        }

        public override string ToString() { return string.Format("{0}_DepartEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Creates an event to start processing a load.
    /// </summary>
    /// <param name="load">The load to be processed.</param>
    /// <returns>A StartEvent to trigger the process.</returns>
    public SimulationEvent Start(TLoad load) { return new StartEvent { AssociatedSandbox = this, Load = load }; }

    /// <summary>
    /// Creates an event to update the departure IsReadyToDepart.
    /// </summary>
    /// <param name="isReadyToDepart">Indicates whether loads should depart.</param>
    /// <returns>An UpdateIsReadyToDepartEvent to trigger the status update.</returns>
    public SimulationEvent UpdateIsReadyToDepart(bool isReadyToDepart)
    {
        return new UpdateIsReadyToDepartEvent
        {
            AssociatedSandbox = this, 
            IsReadyToDepart = isReadyToDepart
        };
    }

    /// <summary>
    /// List of functions that define what happens when loads depart.
    /// </summary>
    public List<Func<TLoad, SimulationEvent>> OnDepart { get; private set; } = new();

    /// <summary>
    /// List of functions that define what happens when the Server state changes.
    /// </summary>
    public List<Func<SimulationEvent>> OnStateChange { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the Server class with a given configuration, seed, and optional tag.
    /// </summary>
    /// <param name="config">The static configuration of the Server.</param>
    /// <param name="seed">The random seed for the simulation.</param>
    /// <param name="tag">An optional tag for identification.</param>
    public Server(ServerStaticConfig<TLoad> config, int seed, string tag) : base(config, seed, "Server", tag) { }

    /// <summary>
    /// Prepares the Server counters to start tracking after the warm-up period.
    /// </summary>
    /// <param name="clockTime">The current simulation time.</param>
    public override void WarmedUp(DateTime clockTime)
    {
        UtilizationCounter.WarmedUp(clockTime);
    }
}