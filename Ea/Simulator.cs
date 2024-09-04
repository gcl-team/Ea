using Ea.Exceptions;
using Ea.RunStrategies;
using Ea.Standard;
using Microsoft.Extensions.Logging;

namespace Ea;

public class Simulator
{
    private readonly ILogger<Simulator> _logger;
    
    public DateTime? RealTimeForLastRun { get; set; }
    public bool HasFutureEvents => FutureEventList.Count > 0;
    public DateTime ClockTime { get; protected internal set; } = DateTime.MinValue;
    public SimulationSandbox Sandbox { get; private set; }
    public DateTime HeadEventTime => HasFutureEvents ? FutureEventList.First().ScheduledTime : DateTime.MaxValue;
    
    internal SortedSet<SimulationEvent> FutureEventList { get; } = new(new FutureEventComparer());
    
    public Simulator(SimulationSandbox sandbox, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Simulator>();
        
        Sandbox = sandbox ?? throw new ArgumentNullException(nameof(sandbox));
        
        foreach (var simulationEvent in Sandbox.SimulationEvents)
        {
            Schedule(simulationEvent, ClockTime);
        }
    }

    public bool Run(IRunStrategy strategy)
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
    
    internal void Schedule(SimulationEvent simulationEvent, DateTime time)
    {
        simulationEvent.Simulator ??= this;

        if (time < ClockTime) throw new SimulationException("Event cannot be scheduled before ClockTime.");

        simulationEvent.ScheduledTime = time;
        FutureEventList.Add(simulationEvent);
    }
    
    internal void Execute(SimulationEvent simulationEvent)
    {
        simulationEvent.Simulator = this;
        simulationEvent.Invoke();
    }

    internal bool ExecuteHeadEvent()
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