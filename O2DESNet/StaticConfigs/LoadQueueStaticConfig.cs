namespace O2DESNet.StaticConfigs;

public class LoadQueueStaticConfig : IStaticConfig
{
    /// <summary>
    /// Maximum number of items that the queue can hold.
    /// </summary>
    public int Capacity { get; set; } = int.MaxValue;
}