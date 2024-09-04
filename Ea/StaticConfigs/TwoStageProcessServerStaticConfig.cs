using Ea.Sandboxes;

namespace Ea.StaticConfigs;

public class TwoStageProcessServerStaticConfig<TLoad> : IStaticConfig
{
    /// <summary>
    /// Static configuration for the HandlingServer.
    /// </summary>
    internal ServerStaticConfig<TLoad> HandlingServer { get; private set; } = new();

    /// <summary>
    /// Static configuration for the RestoringServer.
    /// </summary>
    internal ServerStaticConfig<TLoad> RestoringServer { get; private set; } = new();

    /// <summary>
    /// Time required for the initial handling stage.
    /// </summary>
    public Func<TLoad, Random, TimeSpan> HandlingTime
    {
        get { return HandlingServer.ServiceTime; } 
        set { HandlingServer.ServiceTime = value; }
    }

    /// <summary>
    /// Time required for the restoring stage.
    /// </summary>
    public Func<TLoad, Random, TimeSpan> RestoringTime
    {
        get { return RestoringServer.ServiceTime; } 
        set { RestoringServer.ServiceTime = value; }
    }

    /// <summary>
    /// The total capacity for the handling and restoring stages.
    /// </summary>
    public int Capacity { get; set; }
}