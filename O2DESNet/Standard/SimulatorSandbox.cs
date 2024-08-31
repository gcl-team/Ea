namespace O2DESNet.Standard;

public abstract class SimulatorSandbox : ISimulatorSandbox
{
    protected internal Random DefaultRS { get; private set; }
    private int _seed;
    public int Seed { get { return _seed; } set { _seed = value; DefaultRS = new Random(_seed); } }
    
    public SimulatorSandbox(int seed = 0, string tag = null)
    {
        Seed = seed;
        Display = false;
        Id = ++_count;
        Tag = tag;
    }

    public virtual void WarmedUp(DateTime clockTime) { throw new NotImplementedException(); }

    private class BatchExecuteEvent : SimulationEvent
    {
        internal IEnumerable<SimulationEvent> Events { get; set; }
        public override void Invoke()
        {
            foreach (var e in Events) Execute(e);
        }
    }         
    /// <summary>
    /// Wrap a batch of events as a single event
    /// </summary>
    /// <param name="events">A batch of events</param>
    /// <returns>The single event</returns>
    public SimulationEvent EventWrapper(IEnumerable<SimulationEvent> events) { return new BatchExecuteEvent { Events = events }; }

    #region For Logging
    private string _logFile;
    public bool Display { get; set; }
    public string LogFile
    {
        get { return _logFile; }
        set
        {
            _logFile = value; if (_logFile != null) using (var sw = new StreamWriter(_logFile)) { };
        }
    }
    public void Log(string format, params object[] args)
    {
        if (Display) Console.WriteLine(format, args);
        if (LogFile != null) using (var sw = new StreamWriter(LogFile, true)) sw.WriteLine(format, args);
    }
    public void Log(DateTime clockTime, params object[] args)
    {
        var timeStr = clockTime.ToString("y/M/d H:mm:ss.fff");
        if (Display)
        {
            Console.Write("{0}\t", timeStr);
            foreach (var arg in args) Console.Write("{0}\t", arg);
            Console.WriteLine();
        }
            
        if (LogFile != null)
            using (var sw = new StreamWriter(LogFile, true))
            {
                sw.Write("{0},", timeStr);
                foreach (var arg in args) sw.Write("{0},", arg);
                sw.WriteLine();
            }
    }
    #endregion

    #region Module-based
    protected static int _count = 0;
    public int Id { get; protected set; }
    public string Name { get; protected set; }
    public string Tag { get; set; }        
    public override string ToString()
    {
        if (Tag != null && Tag.Length > 0) return Tag;
        return string.Format("{0}#{1}", Name, Id);
    }
    public List<SimulationEvent> InitEvents { get; } = new();
    #endregion
}

public abstract class SimulatorSandbox<T> : SimulatorSandbox
    where T : SimulationStaticConfig
{
    public T SimulationStaticConfig { get; private set; }
    
    public SimulatorSandbox(T simulationStaticConfig, int seed = 0, string tag = null) : base(seed, tag)
    {
        SimulationStaticConfig = simulationStaticConfig;
    }
}