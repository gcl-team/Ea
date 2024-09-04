namespace Ea.StaticConfigs;

public class ServerStaticConfig<TLoad> : IStaticConfig
{
    /// <summary>
    /// Maximum number of loads the server can handle at one time.
    /// </summary>
    public int Capacity { get; set; } = int.MaxValue;
    
    /// <summary>
    /// Function that defines the service time for each load.
    /// </summary>
    public Func<TLoad, Random, TimeSpan> ServiceTime { get; set; }
}