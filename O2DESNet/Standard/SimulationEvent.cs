namespace O2DESNet.Standard;

public abstract class SimulationEvent
{
    private static int _count = 0;
    internal int Index { get; private set; } = _count++;

    internal SimulationSandbox Sandbox { get; set; } = null;
    internal Simulator? Simulator { get; set; }
    internal protected DateTime ClockTime { get { return Simulator.ClockTime; } }
    public abstract void Invoke();
    protected SimulationEvent() { }
    protected SimulationEvent(Simulator simulator) { Simulator = simulator; }
    internal DateTime ScheduledTime { get; set; }
    internal protected void Log(string format, params object[] args) { Sandbox.Log(ClockTime, string.Format(format, args)); }
    internal protected void Log(params object[] args) { Sandbox.Log(ClockTime, args); }
    protected virtual void Execute(SimulationEvent evnt) { Simulator.Execute(evnt); }
}

public abstract class SimulationEvent<TSandbox, TConfig> : SimulationEvent 
    where TSandbox : SimulationSandbox<TConfig>
    where TConfig : SimulationStaticConfig
{        
    public TSandbox This { get { return (TSandbox)Sandbox; } set { Sandbox = value; } }
    protected TConfig Config { get { return This.SimulationStaticConfig; } }
    protected Random DefaultRS { get { return This.DefaultRS; } }

    protected SimulationEvent() { }
    public SimulationEvent(TSandbox state) { Sandbox = state; }

    private void Induce(SimulationEvent evnt)
    {
        if (evnt.Sandbox == null && Sandbox != null) 
            evnt.Sandbox = Sandbox;
    }

    /// <summary>
    /// Execute an individual event
    /// </summary>
    /// <param name="evnt">The event to be executed</param>
    protected override void Execute(SimulationEvent evnt)
    {
        Induce(evnt); Simulator.Execute(evnt);
    }
    /// <summary>
    /// Execute events in a batch
    /// </summary>
    /// <param name="events">Batch of events to be executed</param>
    protected void Execute(IEnumerable<SimulationEvent> events)
    {
        foreach (var e in events.ToList()) Execute(e);
    }
    protected void Schedule(SimulationEvent evnt, DateTime time) { Induce(evnt); Simulator.Schedule(evnt, time); }
    protected void Schedule(SimulationEvent evnt, TimeSpan delay) { Schedule(evnt, ClockTime + delay); }
    protected void Schedule(SimulationEvent evnt) { Schedule(evnt, ClockTime); }
    protected void Schedule(IEnumerable<SimulationEvent> events)
    {
        foreach (var e in events) Schedule(e);
    }
} 