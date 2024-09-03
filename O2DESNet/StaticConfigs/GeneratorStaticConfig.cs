namespace O2DESNet.StaticConfigs;

public class GeneratorStaticConfig<TLoad> : IStaticConfig
{
    public Func<Random, TimeSpan> InterArrivalTime { get; set; }
    
    public bool IsSkippingFirst { get; set; } = true;
    
    public Func<Random, TLoad> Create { get; set; }
}