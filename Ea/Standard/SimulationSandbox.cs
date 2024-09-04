using Ea.StaticConfigs;

namespace Ea.Standard;

/// <summary>
/// Represents the base class for a simulation sandbox, providing a common structure 
/// and behavior for managing simulation events and randomness.
/// </summary>
public abstract class SimulationSandbox : ISimulatorSandbox
{
    private static int _count;
    private int _seed;
    
    public int Index { get; }
    public string Name { get; }
    public string Tag { get; set; }

    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value; 
            DefaultRs = new Random(_seed); // Reinitialize the RNG with the new seed
        }
    }
    
    /// <summary>
    /// Gets the list of simulation events associated with this sandbox.
    /// </summary>
    public List<SimulationEvent> SimulationEvents { get; } = [];
    
    /// <summary>
    /// Gets the default random number generator, which is initialized with the seed.
    /// </summary>
    protected internal Random DefaultRs { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationSandbox"/> class with the specified
    /// seed, name, and tag.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    /// <param name="name">The name of the sandbox instance.</param>
    /// <param name="tag">The tag associated with the sandbox instance.</param>
    public SimulationSandbox(int seed, string name, string tag)
    {
        Index = ++_count;
        Name = name;
        Tag = tag;
        Seed = seed;
        DefaultRs = new Random(seed);
    }
    
    /// <summary>
    /// Wraps a collection of simulation events into a single simulation event.
    /// </summary>
    /// <param name="events">The collection of simulation events to wrap.</param>
    /// <returns>A new simulation event that represents the batch of events.</returns>
    public SimulationEvent EventWrapper(IEnumerable<SimulationEvent> events)
    {
        return new SimulationEventInBatch { Events = events };
    }

    public abstract void WarmedUp(DateTime clockTime);

    /// <summary>
    /// Returns a string that represents this instance, including the tag if it is not empty.
    /// </summary>
    /// <returns>A string that represents this instance.</returns>
    public override string ToString() => string.IsNullOrWhiteSpace(Tag) ? Tag : $"{Name}#{Index}";
}

/// <summary>
/// Represents a generic simulation sandbox that includes a static configuration of type <typeparamref name="T"/>.
/// Inherits from <see cref="SimulationSandbox"/>.
/// </summary>
/// <typeparam name="T">The type of the static configuration used in the simulation.</typeparam>
public abstract class SimulationSandbox<T>(T simulationStaticConfig, int seed, string name, string tag)
    : SimulationSandbox(seed, name, tag)
    where T : IStaticConfig
{
    /// <summary>
    /// Gets the static configuration for the simulation.
    /// </summary>
    public T SimulationStaticConfig { get; private set; } = simulationStaticConfig;
}