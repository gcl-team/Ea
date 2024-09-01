namespace O2DESNet.Standard;

/// <summary>
/// Tracks and manages count observations over time in hours, along with statistical analysis.
/// </summary>
public class TimeSeriesCounter
{
    private DateTime _initialTime;
    private Dictionary<DateTime, double> _history;
    
    /// <summary>
    /// Indicates whether to keep a history of all count observations.
    /// </summary>
    public bool IsHistoryEnabled { get; private set; }
    /// <summary>
    /// The last recorded time in the analysis.
    /// </summary>
    public DateTime LastRecordedTime { get; private set; }
    /// <summary>
    /// The last observed count value.
    /// </summary>
    public double LastRecordedCount { get; private set; }
    /// <summary>
    /// Total number of increment observed
    /// </summary>
    public double TotalIncrementCount { get; private set; }
    /// <summary>
    /// Total number of decrement observed
    /// </summary>
    public double TotalDecrementCount { get; private set; }
    /// <summary>
    /// Total number of hours since the initial time.
    /// </summary>
    public double TotalHours { get; private set; }
    /// <summary>
    /// The cumulative value of the count over time.
    /// </summary>
    public double CumulativeValue { get; private set; }
    /// <summary>
    /// Indicates whether the analysis is currently paused.
    /// </summary>
    public bool IsPaused { get; private set; }
    
    /// <summary>
    /// The rate of increments observed per hour.
    /// </summary>
    public double IncrementRate => TotalIncrementCount / TotalHours;
    
    /// <summary>
    /// The rate of decrements observed per hour.
    /// </summary>
    public double DecrementRate => TotalDecrementCount / TotalHours;
    
    /// <summary>
    /// A dictionary mapping count values to the total hours observed at each value.
    /// </summary>
    public Dictionary<double, double> HoursForCount { get; private set; } = new();

    /// <summary>
    /// The ratio of working time to the total elapsed time since the initial time.
    /// </summary>
    public double WorkingTimeRatio => LastRecordedTime == _initialTime ? 0 : TotalHours / (LastRecordedTime - _initialTime).TotalHours;

    /// <summary>
    /// The average count over the observation period.
    /// </summary>
    public double AverageCount => TotalHours == 0 ? LastRecordedCount : CumulativeValue / TotalHours;

    /// <summary>
    /// The average timespan that a load stays in the activity, assuming a stationary process.
    /// </summary>
    public TimeSpan AverageTimeSpan
    {
        get
        {
            var hours = AverageCount / DecrementRate;
            if (double.IsNaN(hours) || double.IsInfinity(hours)) hours = 0;
            return TimeSpan.FromHours(hours);
        }
    }
    
    /// <summary>
    /// A list of time-count pairs representing the history of observations.
    /// </summary>
    public List<Tuple<double, double>> History
    {
        get
        {
            if (!IsHistoryEnabled) return null;
            return _history.OrderBy(i => i.Key)
                .Select(i => new Tuple<double, double>((i.Key - _initialTime).TotalHours, i.Value))
                .ToList();
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HourStatisticalAnalysis"/> class with an optional history tracking.
    /// </summary>
    /// <param name="isHistoryEnabled">Whether to keep a history of count observations.</param>
    public TimeSeriesCounter(bool isHistoryEnabled = false) : this(DateTime.MinValue, isHistoryEnabled) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HourStatisticalAnalysis"/> class with a specified initial time and optional history tracking.
    /// </summary>
    /// <param name="initialTime">The initial time for the analysis.</param>
    /// <param name="isHistoryEnabled">Whether to keep a history of count observations.</param>
    public TimeSeriesCounter(DateTime initialTime, bool isHistoryEnabled = false)
    {
        Initialize(initialTime, isHistoryEnabled);
    }

    private void Initialize(DateTime initialTime, bool isHistoryEnabled)
    {
        _initialTime = initialTime;
        LastRecordedTime = initialTime;
        LastRecordedCount = 0;
        TotalIncrementCount = 0;
        TotalDecrementCount = 0;
        TotalHours = 0;
        CumulativeValue = 0;
        IsHistoryEnabled = isHistoryEnabled;
        if (IsHistoryEnabled) _history = new Dictionary<DateTime, double>();
    }
    
    /// <summary>
    /// Records the observed count at a specific time.
    /// </summary>
    public void ObserveCount(double count, DateTime clockTime)
    {
        if (IsPaused) return;

        if (count > LastRecordedCount) 
            TotalIncrementCount += count - LastRecordedCount;
        else 
            TotalDecrementCount += LastRecordedCount - count;

        if (clockTime > LastRecordedTime)
        {
            var hours = (clockTime - LastRecordedTime).TotalHours;
            TotalHours += hours;
            CumulativeValue += hours * LastRecordedCount;
            LastRecordedTime = clockTime;

            if (!HoursForCount.ContainsKey(LastRecordedCount)) 
                HoursForCount.Add(LastRecordedCount, 0);

            HoursForCount[LastRecordedCount] += hours;
        }

        LastRecordedCount = count;

        if (IsHistoryEnabled)
        {
            _history[clockTime] = count;
        }
    }

    /// <summary>
    /// Records a change in the count at a specific time.
    /// </summary>
    /// <param name="change">The change in the count value.</param>
    /// <param name="clockTime">The time at which the change is observed.</param>
    public void ObserveChange(double change, DateTime clockTime)
    {
        ObserveCount(LastRecordedCount + change, clockTime);
    }

    /// <summary>
    /// Pauses the analysis, recording the current time.
    /// </summary>
    public void Pause() 
    { 
        Pause(LastRecordedTime); 
    }

    public void Pause(DateTime clockTime)
    {
        if (IsPaused) return;
        ObserveChange(0, clockTime);
        IsPaused = true;
    }

    /// <summary>
    /// Resumes the analysis from the specified time.
    /// </summary>
    /// <param name="clockTime">The time at which to resume the analysis.</param>
    public void Resume(DateTime clockTime)
    {
        if (!IsPaused) return;
        LastRecordedTime = clockTime;
        IsPaused = false;
    }

    /// <summary>
    /// Warm up the system.
    /// </summary>
    /// <param name="clockTime">The time at which to the simulation is warmed up.</param>
    public void WarmUp(DateTime clockTime)
    {
        _initialTime = clockTime;
        LastRecordedTime = clockTime;
        TotalIncrementCount = 0;
        TotalDecrementCount = 0;
        TotalHours = 0;
        CumulativeValue = 0;
        HoursForCount.Clear();
    }
    
    /// <summary>
    /// Gets the count value at a specified percentile of the time distribution.
    /// </summary>
    /// <param name="percentile">The percentile to evaluate, between 0 and 100.</param>
    /// <returns>The count value at the specified percentile.</returns>
    public double GetPercentile(double percentile)
    {
        SortHoursForCount();
        var threshold = HoursForCount.Sum(i => i.Value) * percentile / 100;
        foreach (var entry in HoursForCount)
        {
            threshold -= entry.Value;
            if (threshold <= 0) return entry.Key;
        }
        return double.PositiveInfinity;
    }

    /// <summary>
    /// Generates a histogram of count values over time.
    /// </summary>
    /// <param name="interval">The width of each count value interval.</param>
    /// <returns>A dictionary mapping the lower bound of each interval to an array containing the total hours observed, probability, and cumulative probability.</returns>
    public Dictionary<double, double[]> GenerateHistogram(double interval)
    {
        SortHoursForCount();
        var histogram = new Dictionary<double, double[]>();
        double cumulativeHours = 0;

        foreach (var entry in HoursForCount)
        {
            if (!histogram.ContainsKey(entry.Key))
            {
                histogram[entry.Key] = new double[3];
            }

            cumulativeHours += entry.Value;

            histogram[entry.Key][0] = cumulativeHours; 
        }

        var totalHours = histogram.Sum(h => h.Value[0]);
        double cumulativeProbability = 0;

        foreach (var entry in histogram)
        {
            var probability = entry.Value[0] / totalHours;
            cumulativeProbability += probability;
            entry.Value[1] = probability;
            entry.Value[2] = cumulativeProbability;
        }

        return histogram;
    }
    
    private void SortHoursForCount()
    {
        HoursForCount = HoursForCount.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);
    }
}