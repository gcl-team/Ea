using Microsoft.Extensions.Logging;
using O2DESNet.Exceptions;
using O2DESNet.Standard;

namespace O2DESNet;

public class Simulator
{
    private ILogger<Simulator> _logger;
    
    internal SortedSet<SimulationEvent> FutureEventList { get; } = new(new FutureEventComparer());
    public DateTime ClockTime { get; protected set; } = DateTime.MinValue;
    public SimulatorSandbox Sandbox { get; private set; }
    public bool HasFutureEvents => FutureEventList.Count > 0;

    public DateTime HeadEventTime => HasFutureEvents ? FutureEventList.First().ScheduledTime : DateTime.MaxValue;
    
    private DateTime? _realTimeForLastRun = null;
    
    public Simulator(SimulatorSandbox sandbox, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Simulator>();
        
        Sandbox = sandbox ?? throw new ArgumentNullException(nameof(sandbox));
        
        foreach (var simulationEvent in Sandbox.InitEvents)
        {
            Schedule(simulationEvent, ClockTime);
        }
    }
    
    public virtual bool Run(SimulationEvent simulationEvent)
    {
        if (simulationEvent.Simulator != null && !simulationEvent.Simulator.Equals(this))
            return false;

        Execute(simulationEvent);
        return true;
    }

    public virtual bool Run(TimeSpan duration) => Run(ClockTime.Add(duration));

    public virtual bool Run(DateTime terminate)
    {
        while (true)
        {
            if (FutureEventList.Count == 0)
                return false;

            if (FutureEventList.First().ScheduledTime <= terminate)
            {
                ExecuteHeadEvent();
            }
            else
            {
                ClockTime = terminate;
                return true;
            }
        }
    }

    public virtual bool Run(int eventCount)
    {
        for (var i = 0; i < eventCount; i++)
        {
            if (!ExecuteHeadEvent())
                return false;
        }
        return true;
    }

    public virtual bool Run(double speed)
    {
        if (_realTimeForLastRun == null)
        {
            // If this is the first run, just update the last run time.
            _realTimeForLastRun = DateTime.Now;
            return true;
        }
        
        TimeSpan elapsedRealTime = DateTime.Now - _realTimeForLastRun.Value;
        DateTime targetTime = ClockTime.AddSeconds(elapsedRealTime.TotalSeconds * speed);

        bool result = Run(targetTime);
        _realTimeForLastRun = DateTime.Now;

        return result;
    }

    public bool WarmUp(TimeSpan duration)
    {
        var result = Run(duration);
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

    protected bool ExecuteHeadEvent()
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