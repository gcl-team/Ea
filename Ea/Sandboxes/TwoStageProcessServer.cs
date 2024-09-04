using Ea.Exceptions;
using Ea.Standard;
using Ea.StaticConfigs;

namespace Ea.Sandboxes;

/// <summary>
/// Represents a server with two distinct processing stages: Handling and Restoring.
/// </summary>
/// <typeparam name="TLoad">The type of load that the server handles.</typeparam>
public class TwoStageProcessServer<TLoad> : SimulationSandbox<TwoStageProcessServerStaticConfig<TLoad>>
{
    /// <summary>
    /// Server that handles the initial processing stage of the load.
    /// </summary>
    internal Server<TLoad> HandlingServer { get; private set; }

    /// <summary>
    /// Server that handles the restoring stage of the load after the initial processing.
    /// </summary>
    internal Server<TLoad> RestoringServer { get; private set; }
    
    /// <summary>
    /// Loads currently being processed in the handling stage.
    /// </summary>
    public HashSet<TLoad> HandlingLoads { get { return HandlingServer.Serving; } }

    /// <summary>
    /// Loads that have completed the handling stage and are awaiting the restoring stage.
    /// </summary>
    public HashSet<TLoad> HandledLoads { get { return HandlingServer.Served; } }

    /// <summary>
    /// Loads currently being processed in the restoring stage.
    /// </summary>
    public HashSet<TLoad> RestoringLoads { get { return RestoringServer.Serving; } }

    /// <summary>
    /// The number of available spaces in the server for new loads.
    /// </summary>
    public int Vacancy { get { return SimulationStaticConfig.Capacity - Occupancy; } }

    /// <summary>
    /// The total number of loads that have completed the handling stage.
    /// </summary>
    public int HandledLoadsCount { get { return (int)HandlingServer.UtilizationCounter.TotalDecrementCount; } }

    /// <summary>
    /// The total number of loads currently in the server, including both stages.
    /// </summary>
    public int Occupancy => HandlingLoads.Count + HandledLoads.Count + RestoringLoads.Count;

    /// <summary>
    /// The utilization rate of the server, calculated based on both stages.
    /// </summary>
    public double Utilization => (HandlingServer.UtilizationCounter.AverageCount + RestoringServer.UtilizationCounter.AverageCount) / SimulationStaticConfig.Capacity;

    /// <summary>
    /// The occupation rate of the server, calculated based on both stages.
    /// </summary>
    public double Occupation => (HandlingServer.OccupationCounter.AverageCount + RestoringServer.OccupationCounter.AverageCount) / SimulationStaticConfig.Capacity;

    /// <summary>
    /// The effective hourly rate of load processing, calculated based on the handling stage.
    /// </summary>
    public double EffectiveHourlyRate => HandlingServer.UtilizationCounter.DecrementRate;

    /// <summary>
    /// Indicates whether the server is set to ready for departures after processing.
    /// </summary>
    public bool IsReadyToDepart => HandlingServer.IsReadyToDepart;
    
    /// <summary>
    /// Base class for internal events within the <see cref="TwoStageProcessServer{TLoad}"/>.
    /// </summary>
    private abstract class InternalEvent : SimulationEvent<TwoStageProcessServer<TLoad>, TwoStageProcessServerStaticConfig<TLoad>> { }

    /// <summary>
    /// Event representing the start of processing for a load in the handling stage.
    /// </summary>
    private class StartEvent : InternalEvent
    {
        internal TLoad Load { get; set; }
        public override void Invoke()
        {
            if (AssociatedSandbox.Vacancy < 1) throw new SimulationException("The Server vacancy cannot be zero before starting a load.");
            Execute(AssociatedSandbox.HandlingServer.Start(Load));
            Execute(new StateChangeEvent());
        }
        public override string ToString() { return string.Format("{0}_StartEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Event representing a state change within the server.
    /// </summary>
    private class StateChangeEvent : InternalEvent
    {
        public override void Invoke() { Execute(AssociatedSandbox.OnStateChange.Select(e => e())); }
        public override string ToString() { return string.Format("{0}_StateChangeEvent", AssociatedSandbox); }
    }

    /// <summary>
    /// Triggers the start of processing for a load.
    /// </summary>
    /// <param name="load">The load to be processed.</param>
    /// <returns>An event representing the start of processing.</returns>
    public SimulationEvent Start(TLoad load)
    {
        return new StartEvent { AssociatedSandbox = this, Load = load };
    }

    /// <summary>
    /// Updates whether the server should allow departures after processing.
    /// </summary>
    /// <param name="isReadyToDepart">True to allow departures, false to prevent them.</param>
    /// <returns>An event representing the update.</returns>
    public SimulationEvent UpdateIsReadyToDepart(bool isReadyToDepart)
    {
        return HandlingServer.UpdateIsReadyToDepart(isReadyToDepart);
    }
    
    /// <summary>
    /// Events triggered when a load departs from the handling stage.
    /// </summary>
    public List<Func<TLoad, SimulationEvent>> OnHandlingStageDepart => HandlingServer.OnDepart;

    /// <summary>
    /// Events triggered when a load departs from the restoring stage.
    /// </summary>
    public List<Func<TLoad, SimulationEvent>> OnRestoringStageDepart => RestoringServer.OnDepart;

    /// <summary>
    /// Events triggered when the state of the server changes.
    /// </summary>
    public List<Func<SimulationEvent>> OnStateChange { get; private set; } = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoStageServer{TLoad}"/> class.
    /// </summary>
    /// <param name="config">The configuration for the server.</param>
    /// <param name="seed">The random seed for simulation.</param>
    /// <param name="tag">An optional tag for identifying the server instance.</param>
    public TwoStageProcessServer(TwoStageProcessServerStaticConfig<TLoad> config, int seed, string tag) : 
        base(config, seed, "TwoStageServer", tag)
    {
        HandlingServer = new Server<TLoad>(config.HandlingServer, DefaultRs.Next(), tag);
        RestoringServer = new Server<TLoad>(config.RestoringServer, DefaultRs.Next(), tag);

        HandlingServer.OnDepart.Add(RestoringServer.Start);
        HandlingServer.OnStateChange.Add(() => new StateChangeEvent { AssociatedSandbox = this });
        RestoringServer.OnStateChange.Add(() => new StateChangeEvent { AssociatedSandbox = this });
    }

    /// <summary>
    /// Warms up the server to prepare it for operation, based on the provided clock time.
    /// </summary>
    /// <param name="clockTime">The current clock time for the simulation.</param>
    public override void WarmedUp(DateTime clockTime)
    {
        HandlingServer.WarmedUp(clockTime);
        RestoringServer.WarmedUp(clockTime);
    }
}