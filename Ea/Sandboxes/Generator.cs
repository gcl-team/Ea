using Ea.Exceptions;
using Ea.Standard;
using Ea.StaticConfigs;

namespace Ea.Sandboxes;

/// <summary>
/// Represents a generator that produces loads of type <typeparamref name="TLoad"/>
/// based on specified configurations.
/// </summary>
/// <typeparam name="TLoad">The type of load generated by this generator.</typeparam>
public class Generator<TLoad> : SimulationSandboxBase<GeneratorStaticConfig<TLoad>>
{
    /// <summary>
    /// Gets the start time of the generator.
    /// </summary>
    public DateTime? StartTime { get; private set; }
    
    /// <summary>
    /// Gets a value indicating whether the generator is currently active.
    /// </summary>
    public bool IsActivated { get; private set; }
    
    /// <summary>
    /// Gets the number of loads generated by the generator.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Creates a new <see cref="StartEvent"/> to start the generator.
    /// </summary>
    /// <returns>A <see cref="StartEvent"/> to start the generator.</returns>
    public SimulationEventBase Start() => new StartEvent { AssociatedSandbox = this };
    
    /// <summary>
    /// Creates a new <see cref="EndEvent"/> to stop the generator.
    /// </summary>
    /// <returns>A <see cref="EndEvent"/> to stop the generator.</returns>
    public SimulationEventBase End() => new EndEvent { AssociatedSandbox = this };

    /// <summary>
    /// Gets a list of functions to execute when a load arrives.
    /// Each function returns an event that will be scheduled when the load arrives.
    /// </summary>
    public List<Func<TLoad, SimulationEventBase>> OnArrive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Generator{TLoad}"/> class with the
    /// specified configuration, seed, and optional tag.
    /// </summary>
    /// <param name="config">The configuration settings for the generator.</param>
    /// <param name="seed">The seed value for random number generation.</param>
    /// <param name="tag">An optional tag for the generator.</param>
    public Generator(GeneratorStaticConfig<TLoad> config, int seed, string tag) : base(config, seed, "Generator", tag)
    {
        IsActivated = false;
        Count = 0;

        OnArrive = new List<Func<TLoad, SimulationEventBase>>();
    }

    public override void WarmedUp(DateTime clockTime)
    {
        StartTime = clockTime;
        Count = 0;
    }
    
    private abstract class InternalEvent : SimulationEventBase<Generator<TLoad>, GeneratorStaticConfig<TLoad>> { }
    
    private sealed class StartEvent : InternalEvent
    {
        public override void Invoke()
        {
            if (!AssociatedSandbox.IsActivated)
            {
                if (Config.InterArrivalTime == null) 
                    throw new SimulationException("The InterArrivalTime in the Generator is not configured.");
                
                AssociatedSandbox.IsActivated = true;
                AssociatedSandbox.StartTime = ClockTime;
                AssociatedSandbox.Count = 0;

                if (Config.IsSkippingFirst)
                {
                    Schedule(new ArriveEvent(), Config.InterArrivalTime(DefaultRs));
                }
                else
                {
                    Schedule(new ArriveEvent());
                }
            }
        }
    }

    private sealed class EndEvent : InternalEvent
    {
        public override void Invoke()
        {
            if (AssociatedSandbox.IsActivated)
            {
                AssociatedSandbox.IsActivated = false;
            }
        }
    }

    private sealed class ArriveEvent : InternalEvent
    {
        public override void Invoke()
        {
            if (AssociatedSandbox.IsActivated)
            {
                var load = Config.Create(DefaultRs);
                AssociatedSandbox.Count++;
                Schedule(new ArriveEvent(), Config.InterArrivalTime(DefaultRs));
                Execute(AssociatedSandbox.OnArrive.Select(e => e(load)));
            }
        }

        public override string ToString() => $"{AssociatedSandbox}_Arrive";
    }
}