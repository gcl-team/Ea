using Ea.Exceptions;
using Ea.Standard;
using Ea.StaticConfigs;

namespace Ea.Sandboxes;

/// <summary>
/// Represents a queueing system where items can be enqueued and dequeued.
/// </summary>
/// <typeparam name="TLoad">The type of load that the queue can hold.</typeparam>
public class LoadQueue<TLoad> : SimulationSandbox<LoadQueueStaticConfig>
{
    /// <summary>
    /// List of items currently in the queue.
    /// </summary>
    public List<TLoad> Waiting { get; private set; } = new List<TLoad>();

    /// <summary>
    /// Current number of items in the queue.
    /// </summary>
    public int Occupancy => Waiting.Count;

    /// <summary>
    /// Available space in the queue.
    /// </summary>
    public int Vacancy => SimulationStaticConfig.Capacity - Occupancy;

    /// <summary>
    /// Indicates whether the queue is set to dequeue items.
    /// </summary>
    public bool ToDequeue { get; private set; } = true;

    /// <summary>
    /// Hourly statistics of the queue.
    /// </summary>
    public TimeBasedMetric TimeBasedMetric { get; private set; } = new();

    /// <summary>
    /// Utilization rate of the queue.
    /// </summary>
    public double Utilization => TimeBasedMetric.AverageCount / SimulationStaticConfig.Capacity;
    
    /// <summary>
    /// Base class for internal events in the queueing system.
    /// </summary>
    private abstract class InternalEvent : SimulationEvent<LoadQueue<TLoad>, LoadQueueStaticConfig> { }

    /// <summary>
    /// Event for enqueuing an item into the queue.
    /// </summary>
    private sealed class EnqueueEvent : InternalEvent
    {
        internal TLoad Load { private get; set; }

        public override void Invoke()
        {
            if (AssociatedSandbox.Vacancy == 0)
                throw new SimulationException("The queue has no available vacancy for the item.");

            AssociatedSandbox.Waiting.Add(Load);
            AssociatedSandbox.TimeBasedMetric.ObserveChange(1, ClockTime);
            Execute(new StateChangeEvent());

            if (AssociatedSandbox.ToDequeue)
                Execute(new DequeueEvent());
        }

        public override string ToString() => $"{AssociatedSandbox}_Enqueue";
    }

    /// <summary>
    /// Event for updating the dequeue status of the queue.
    /// </summary>
    private sealed class UpdateToDequeueEvent : InternalEvent
    {
        internal bool ToDequeue { private get; set; }

        public override void Invoke()
        {
            AssociatedSandbox.ToDequeue = ToDequeue;

            if (AssociatedSandbox.ToDequeue && AssociatedSandbox.Waiting.Count > 0)
                Execute(new DequeueEvent());
        }

        public override string ToString() => $"{AssociatedSandbox}_UpdateToDequeue";
    }

    /// <summary>
    /// Event for handling state changes in the queue.
    /// </summary>
    private sealed class StateChangeEvent : InternalEvent
    {
        public override void Invoke()
        {
            Execute(AssociatedSandbox.OnStateChange.Select(e => e()));
        }

        public override string ToString() => $"{AssociatedSandbox}_StateChange";
    }

    /// <summary>
    /// Event for dequeuing the first item from the queue.
    /// </summary>
    private sealed class DequeueEvent : InternalEvent
    {
        public override void Invoke()
        {
            TLoad load = AssociatedSandbox.Waiting.FirstOrDefault();

            if (load != null)
            {
                AssociatedSandbox.Waiting.RemoveAt(0);
                AssociatedSandbox.TimeBasedMetric.ObserveChange(-1, ClockTime);

                foreach (var evt in AssociatedSandbox.OnDequeue)
                    Execute(evt(load));

                Execute(new StateChangeEvent());

                if (AssociatedSandbox.ToDequeue && AssociatedSandbox.Waiting.Count > 0)
                    Execute(new DequeueEvent());
            }
        }

        public override string ToString() => $"{AssociatedSandbox}_Dequeue";
    }
    
    /// <summary>
    /// Creates an event to enqueue an item.
    /// </summary>
    /// <param name="load">The item to enqueue.</param>
    /// <returns>An event for enqueuing the item.</returns>
    public SimulationEvent Enqueue(TLoad load) => new EnqueueEvent
    {
        AssociatedSandbox = this, 
        Load = load
    };

    /// <summary>
    /// Creates an event to update the dequeue status.
    /// </summary>
    /// <param name="toDequeue">The new dequeue status.</param>
    /// <returns>An event for updating the dequeue status.</returns>
    public SimulationEvent UpdateToDequeue(bool toDequeue) => new UpdateToDequeueEvent
    {
        AssociatedSandbox = this, 
        ToDequeue = toDequeue
    };
    
    
    /// <summary>
    /// Actions to perform when an item is dequeued.
    /// </summary>
    public List<Func<TLoad, SimulationEvent>> OnDequeue { get; private set; } = new List<Func<TLoad, SimulationEvent>>();
    
    /// <summary>
    /// Actions to perform when the state changes.
    /// </summary>
    public List<Func<SimulationEvent>> OnStateChange { get; private set; } = new List<Func<SimulationEvent>>();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Queueing{TLoad}"/> class with default settings.
    /// </summary>
    public LoadQueue() : base(new LoadQueueStaticConfig(), 0, "LoadQueue", "") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Queueing{TLoad}"/> class with specified configuration.
    /// </summary>
    /// <param name="config">The configuration settings for the queueing system.</param>
    /// <param name="tag">An optional tag for the instance.</param>
    public LoadQueue(LoadQueueStaticConfig config, string tag) : base(config, 0, "LoadQueue", tag) { }

    /// <summary>
    /// Called when the system is warmed up.
    /// </summary>
    /// <param name="clockTime">The current time of the clock.</param>
    public override void WarmedUp(DateTime clockTime)
    {
        TimeBasedMetric.WarmedUp(clockTime);
    }

}