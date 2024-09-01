namespace O2DESNet.Standard;

/// <summary>
/// Tracks the duration of different states of an entity within a simulation.
/// </summary>
/// <typeparam name="TState">The type of the state being tracked.</typeparam>
public class StateTracker<TState> where TState : Enum
{
    private DateTime _initialTime;

    /// <summary>
    /// Gets or sets the last recorded time.
    /// </summary>
    public DateTime LastRecordedTime { get; private set; }

    /// <summary>
    /// Gets the last recorded state.
    /// </summary>
    public TState LastRecordedState { get; private set; }

    /// <summary>
    /// Gets the history of state changes, if history tracking is enabled.
    /// </summary>
    public List<Tuple<DateTime, TState>> History { get; private set; }

    /// <summary>
    /// Gets a value indicating whether history tracking is enabled.
    /// </summary>
    public bool IsHistoryEnabled { get; private set; }

    /// <summary>
    /// Gets the total duration spent in each state.
    /// </summary>
    public Dictionary<TState, TimeSpan> StateDurations { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateTracker{TState}"/> class.
    /// </summary>
    /// <param name="initialState">The initial state to start tracking from.</param>
    /// <param name="isHistoryEnabled">Indicates whether to track the history of state changes.</param>
    public StateTracker(TState initialState, bool isHistoryEnabled = false)
    {
        History = [];
        LastRecordedTime = _initialTime;
        LastRecordedState = initialState;
        IsHistoryEnabled = isHistoryEnabled;
        
        if (IsHistoryEnabled)
        {
            History = new List<Tuple<DateTime, TState>>
            {
                new(LastRecordedTime, LastRecordedState)
            };
        }
        
        StateDurations = new Dictionary<TState, TimeSpan>();
    }
    
    /// <summary>
    /// Updates the tracker with a new state and the corresponding time.
    /// </summary>
    /// <param name="state">The new state to transition to.</param>
    /// <param name="clockTime">The time of the state transition.</param>
    public void UpdateState(TState state, DateTime clockTime)
    {
        if (IsHistoryEnabled)
        {
            History.Add(new Tuple<DateTime, TState>(clockTime, state));
        }
        
        var stateDuration = clockTime - LastRecordedTime;
        if (!StateDurations.TryAdd(LastRecordedState, stateDuration))
        {
            StateDurations[LastRecordedState] += stateDuration;
        }
        
        LastRecordedState = state;
        LastRecordedTime = clockTime;
    }
    
    /// <summary>
    /// Resets the tracker with a new start time.
    /// </summary>
    /// <param name="clockTime">The time to start tracking from.</param>
    public void WarmedUp(DateTime clockTime)
    {
        _initialTime = clockTime;
        LastRecordedTime = clockTime;
        
        if (IsHistoryEnabled)
        {
            History = new List<Tuple<DateTime, TState>>
            {
                new(clockTime, LastRecordedState)
            };
        }
        
        StateDurations = new Dictionary<TState, TimeSpan>();
    }
    
    /// <summary>
    /// Gets the proportion of time spent in a specific state.
    /// </summary>
    /// <param name="state">The state to calculate the proportion for.</param>
    /// <param name="clockTime">The current simulation time.</param>
    /// <returns>The proportion of time spent in the specified state.</returns>
    public double GetProportion(TState state, DateTime clockTime)
    {
        var timespan = !StateDurations.TryGetValue(state, out var value) ? 0 : value.TotalHours;
        
        if (state.Equals(LastRecordedState))
        {
            timespan += (clockTime - LastRecordedTime).TotalHours;
        }
        
        var totalDuration = (clockTime - _initialTime).TotalHours;
        
        return timespan / totalDuration;
    }
}