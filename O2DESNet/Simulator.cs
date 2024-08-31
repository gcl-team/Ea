using Microsoft.Extensions.Logging;
using O2DESNet.Exceptions;
using O2DESNet.RunStrategies;
using O2DESNet.Standard;

namespace O2DESNet;

public class Simulator
{
    private readonly ILogger<Simulator> _logger;
    public DateTime? RealTimeForLastRun = null;
    public bool HasFutureEvents => FutureEventList.Count > 0;
    internal SortedSet<SimulationEvent> FutureEventList { get; } = new(new FutureEventComparer());
    public DateTime ClockTime { get; protected internal set; } = DateTime.MinValue;
    public SimulatorSandbox Sandbox { get; private set; }
    public DateTime HeadEventTime => HasFutureEvents ? FutureEventList.First().ScheduledTime : DateTime.MaxValue;
    
    public Simulator(SimulatorSandbox sandbox, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Simulator>();
        
        Sandbox = sandbox ?? throw new ArgumentNullException(nameof(sandbox));
        
        foreach (var simulationEvent in Sandbox.InitEvents)
        {
            Schedule(simulationEvent, ClockTime);
        }
    }

    public virtual bool Run(IRunStrategy strategy)
    {
        return strategy.Run(this);
    }

    public bool WarmUp(TimeSpan duration)
    {
        var runByDurationStrategy = new RunByDurationStrategy(duration);
        var result = Run(runByDurationStrategy);
        Sandbox.WarmedUp(ClockTime);
        return result;
    }
    
    protected internal void Execute(SimulationEvent simulationEvent)
    {
        simulationEvent.Simulator = this;
        simulationEvent.Invoke();
    }
    
    protected internal void Schedule(SimulationEvent simulationEvent, DateTime time)
    {
        simulationEvent.Simulator ??= this;

        if (time < ClockTime) throw new SimulationException("Event cannot be scheduled before ClockTime.");

        simulationEvent.ScheduledTime = time;
        FutureEventList.Add(simulationEvent);
    }

    protected internal bool ExecuteHeadEvent()
    {
        var head = FutureEventList.FirstOrDefault();
        if (head == null)
            return false;

        FutureEventList.Remove(head);
        ClockTime = head.ScheduledTime;
        head.Invoke();
        return true;
    }
}