namespace O2DESNet.Standard;

/// <summary>
/// Tracks time-based metrics including hourly counts, increments, and decrements over time.
/// Provides statistical calculations based on the observed data.
/// </summary>
public class TimeBasedMetric
{
   
    private DateTime _initialTime;
    private Dictionary<DateTime, double> _history;
    
    public DateTime LastRecordedTime;
    
    public double LastCount { get; private set; }
    
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
    
    public double WorkingTimeRatio
    {
        get
        {
            if (LastRecordedTime == _initialTime) return 0;
            return TotalHours / (LastRecordedTime - _initialTime).TotalHours;
        }
    }
    
    /// <summary>
    /// The cumulative count value on time in unit of hours
    /// </summary>
    public double CumulativeCount { get; private set; }
    
    /// <summary>
    /// The average count on observation period
    /// </summary>
    public double AverageCount { get { if (TotalHours == 0) return LastCount; return CumulativeCount / TotalHours; } }
    
    /// <summary>
    /// Average timespan that a load stays in the activity, if it is a stationary process, 
    /// i.e., decrement rate == increment rate
    /// It is 0 at the initial status, i.e., decrement rate is NaN (no decrement observed).
    /// </summary>
    public TimeSpan AverageTimeSpan
    {
        get
        {
            double hours = AverageCount / DecrementRate;
            if (double.IsNaN(hours) || double.IsInfinity(hours)) hours = 0;
            return TimeSpan.FromHours(hours);
        }
    }
    
    public bool IsPaused { get; private set; }

    
    public bool IsHistoryEnabled { get; private set; }
    
    /// <summary>
    /// Scatter points of (time in hours, count)
    /// </summary>
    public List<Tuple<double, double>> History
    {
        get
        {
            if (!IsHistoryEnabled) return null;
            return _history.OrderBy(i => i.Key).Select(i => new Tuple<double, double>((i.Key - _initialTime).TotalHours, i.Value)).ToList();
        }
    }

    public TimeBasedMetric(bool isHistoryEnabled = false)
    {
        Init(DateTime.MinValue, isHistoryEnabled);
    }

    public TimeBasedMetric(DateTime initialTime, bool isHistoryEnabled = false)
    {
        Init(initialTime, isHistoryEnabled);
    }
    
    private void Init(DateTime initialTime, bool isHistoryEnabled)
    {
        _initialTime = initialTime;
        LastRecordedTime = initialTime;
        LastCount = 0;
        TotalIncrementCount = 0;
        TotalDecrementCount = 0;
        TotalHours = 0;
        CumulativeCount = 0;
        IsHistoryEnabled = isHistoryEnabled;
        
        if (isHistoryEnabled)
        {
            _history = new Dictionary<DateTime, double>();
        }
    }
    
    public void ObserveCount(double count, DateTime clockTime)
    {
        if (IsPaused) return;

        if (count > LastCount)
        {
            TotalIncrementCount += count - LastCount;
        }
        else
        {
            TotalDecrementCount += LastCount - count;
        }

        if (clockTime > LastRecordedTime)
        {
            var hours = (clockTime - LastRecordedTime).TotalHours;
            TotalHours += hours;
            CumulativeCount += hours * LastCount;
            LastRecordedTime = clockTime;

            if (!HoursForCount.ContainsKey(LastCount)) HoursForCount.Add(LastCount, 0);
            HoursForCount[LastCount] += hours;
        }
        
        LastCount = count;

        if (IsHistoryEnabled)
        {
            _history[clockTime] = count;
        }
    }

    public void ObserveChange(double change, DateTime clockTime)
    {
        ObserveCount(LastCount + change, clockTime);
    }

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
    
    public void Resume(DateTime clockTime)
    {
        if (!IsPaused) return;
        LastRecordedTime = clockTime;
        IsPaused = false;
    }

    public double IncrementRate => TotalIncrementCount / TotalHours;
    public double DecrementRate => TotalDecrementCount / TotalHours;

    public void WarmedUp(DateTime clockTime)
    {
        _initialTime = clockTime;
        LastRecordedTime = clockTime;
        TotalIncrementCount = 0;
        TotalDecrementCount = 0;
        TotalHours = 0;
        CumulativeCount = 0;
        HoursForCount = new Dictionary<double, double>();
    }

    public Dictionary<double, double> HoursForCount = new();

    private void SortHoursForCount()
    {
        HoursForCount = HoursForCount
            .OrderBy(i => i.Key)
            .ToDictionary(i => i.Key, i => i.Value);
    }
    
    /// <summary>
    /// Get the percentile of count values on time, i.e., the count value that with x-percent of time the observation is not higher than it.
    /// </summary>
    /// <param name="ratio">values between 0 and 100</param>
    public double Percentile(double ratio)
    {
        SortHoursForCount();
        
        var threashold = HoursForCount.Sum(i => i.Value) * ratio / 100;
        
        foreach (var i in HoursForCount)
        {
            threashold -= i.Value;
            if (threashold <= 0) return i.Key;
        }
        
        return double.PositiveInfinity;
    }
    
    /// <summary>
    /// Statistics for the amount of time spent at each range of count values
    /// </summary>
    /// <param name="countInterval">width of the count value interval</param>
    /// <returns>A dictionary map from [the lowerbound value of each interval] to the array of [total hours observed], [probability], [cumulated probability]</returns>
    public Dictionary<double, double[]> GenerateHistogram(double countInterval) // interval -> { observation, probability, cumulative probability}
    {
        SortHoursForCount();
        var histogram = new Dictionary<double, double[]>();
        if (HoursForCount.Count > 0)
        {
            double countLb = 0;
            double cumHours = 0;
            foreach (var i in HoursForCount)
            {
                if (i.Key > countLb + countInterval || i.Equals(HoursForCount.Last()))
                {
                    if (cumHours > 0) histogram.Add(countLb, new double[] { cumHours, 0, 0 });
                    countLb += countInterval;
                    cumHours = i.Value;
                }
                else
                {
                    cumHours += i.Value;
                }
            }
        }
        
        var sum = histogram.Sum(h => h.Value[0]);
        
        double cum = 0;
        
        foreach (var h in histogram)
        {
            cum += h.Value[0];
            h.Value[1] = h.Value[0] / sum; // probability
            h.Value[2] = cum / sum; // cum. prob.
        }
        
        return histogram;
    }
    
}