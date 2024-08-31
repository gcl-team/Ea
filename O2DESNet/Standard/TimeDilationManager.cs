namespace O2DESNet.Standard;

/// <summary>
/// Time dilation is a concept used in discrete event simulation to scale
/// the simulation time relative to real time.
/// </summary>
public class TimeDilationManager
{
    private DateTime _realTimeAtDilationReset;
    private DateTime _dilatedTimeAtDilationScaleReset;
    private double _timeDilationScale;

    public double TimeDilationScale => _timeDilationScale;

    public TimeDilationManager(double scale, DateTime realClockTime)
    {
        if (scale <= 0) throw new ArgumentException("Scale must be positive.", nameof(scale));

        _realTimeAtDilationReset = realClockTime;
        _timeDilationScale = scale;
    }

    public void UpdateTimeDilationScale(double scale, DateTime realClockTime, DateTime dilatedClockTime)
    {
        if (scale <= 0) throw new ArgumentException("Scale must be positive.", nameof(scale));

        _dilatedTimeAtDilationScaleReset = dilatedClockTime;
        _realTimeAtDilationReset = realClockTime;
        _timeDilationScale = scale;
    }

    public DateTime ConvertRealTimeToDilatedTime(DateTime realTime)
    {
        return _dilatedTimeAtDilationScaleReset +
               TimeSpan.FromSeconds((realTime - _realTimeAtDilationReset).TotalSeconds * _timeDilationScale);
    }

    public DateTime ConvertDilatedTimeToRealTime(DateTime dilatedTime)
    {
        return _realTimeAtDilationReset +
               TimeSpan.FromSeconds((dilatedTime - _dilatedTimeAtDilationScaleReset).TotalSeconds / _timeDilationScale);
    }
}