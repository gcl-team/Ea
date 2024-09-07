using Ea.Exceptions;
using Ea.StaticConfigs;
using System.Diagnostics.CodeAnalysis;

namespace Ea.Standard;

/// <summary>
/// Represents a base class for all events in the simulation.
/// </summary>
[SuppressMessage("NDepend", "ND2102:AvoidDefiningMultipleTypesInASourceFile", Justification = "SimulationEvent<TSandbox, TConfig> is relatively small and closely linked to SimulationEvent")]
public abstract class SimulationEvent
{
    private static int _count = 0;
    
    /// <summary>
    /// Gets the unique index of this event, assigned at creation.
    /// </summary>
    internal int Index { get; private set; } = _count++;
    /// <summary>
    /// Gets or sets the sandbox that contains this event.
    /// </summary>
    internal SimulationSandbox Sandbox { get; set; }
    /// <summary>
    /// Gets or sets the simulator associated with this event. 
    /// It may be null if not yet assigned.
    /// </summary>
    internal Simulator? Simulator { get; set; }
    /// <summary>
    /// Gets the current clock time from the associated simulator. 
    /// This represents the real-time equivalent in the simulation.
    /// </summary>
    internal protected DateTime ClockTime => Simulator?.ClockTime ?? 
                                             throw new SimulationException("Simulator is not assigned.");
    /// <summary>
    /// The scheduled time for this event to be executed.
    /// </summary>
    internal DateTime ScheduledTime { get; set; }
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationEvent"/> class.
    /// </summary>
    protected SimulationEvent() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationEvent"/> class 
    /// with an associated simulator.
    /// </summary>
    /// <param name="simulator">The simulator to associate with this event.</param>
    protected SimulationEvent(Simulator simulator)
    {
        Simulator = simulator;
    }
    
    /// <summary>
    /// Abstract method that must be implemented by derived classes to define 
    /// the event's behavior when it is invoked.
    /// </summary>
    public abstract void Invoke();
    
    /// <summary>
    /// Executes the given event by delegating the execution to the associated simulator.
    /// </summary>
    /// <param name="simulationEvent">The event to execute.</param>
    protected virtual void Execute(SimulationEvent simulationEvent) 
    { 
        if (Simulator == null) 
            throw new SimulationException("Simulator is not assigned.");
        
        Simulator.Execute(simulationEvent); 
    }
}

/// <summary>
/// Represents a specialized event in the simulation that is associated with a 
/// specific sandbox and configuration. This class extends the base <see cref="SimulationEvent"/> 
/// class to handle events with additional type parameters for sandbox and configuration.
/// </summary>
/// <typeparam name="TSandbox">The type of the sandbox associated with this event.</typeparam>
/// <typeparam name="TConfig">The type of the configuration used by the sandbox.</typeparam>
public abstract class SimulationEvent<TSandbox, TConfig> : SimulationEvent 
    where TSandbox : SimulationSandbox<TConfig>
    where TConfig : IStaticConfig
{        
    /// <summary>
    /// Gets or sets the sandbox associated with this event. 
    /// </summary>
    public TSandbox AssociatedSandbox 
    { 
        get => (TSandbox)Sandbox;
        set => Sandbox = value;
    }
    
    /// <summary>
    /// Gets the configuration associated with the sandbox.
    /// </summary>
    protected TConfig Config => AssociatedSandbox.SimulationStaticConfig;
    
    /// <summary>
    /// Gets the default random number generator used by the sandbox.
    /// </summary>
    protected Random DefaultRs => AssociatedSandbox.DefaultRs;

    protected SimulationEvent() { }

    public SimulationEvent(TSandbox sandbox)
    {
        Sandbox = sandbox;
    }
    
    /// <summary>
    /// Execute events in a batch
    /// </summary>
    /// <param name="simulationEvent">Batch of events to be executed</param>
    protected void Execute(IEnumerable<SimulationEvent> simulationEvent)
    {
        foreach (var simulationEven in simulationEvent.ToList())
        {
            Execute(simulationEvent);
        }
    }

    protected void Schedule(SimulationEvent simulationEvent, DateTime time)
    {
        if (Simulator == null) 
            throw new SimulationException("Simulator is not assigned.");
        
        Simulator.Schedule(simulationEvent, time);
    }

    protected void Schedule(SimulationEvent simulationEvent, TimeSpan delay)
    {
        Schedule(simulationEvent, ClockTime + delay);
    }

    protected void Schedule(SimulationEvent simulationEvent)
    {
        Schedule(simulationEvent, ClockTime);
    }
    
    protected void Schedule(IEnumerable<SimulationEvent> simulationEvents)
    {
        foreach (var simulationEvent in simulationEvents)
        {
            Schedule(simulationEvent);
        }
    }
} 