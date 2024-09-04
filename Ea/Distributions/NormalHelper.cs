namespace Ea.Distributions;

/// <summary>
/// Provides methods for sampling from a normal distribution.
/// </summary>
public static class NormalHelper
{
    /// <summary>
    /// Generates a sample from a normal distribution.
    /// </summary>
    /// <param name="rs">Random number generator.</param>
    /// <param name="mean">The mean of the normal distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <returns>A sample from the normal distribution.</returns>
    public static double Sample(Random rs, double mean, double cv)
    {
        ValidateParameters(mean, cv, true);
        
        if (mean == 0 || cv == 0) return mean;
        
        var stddev = cv * mean;
        return MathNet.Numerics.Distributions.Normal.Sample(rs, mean, stddev);
    }
    
    /// <summary>
    /// Computes the cumulative distribution function (CDF) of a normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the normal distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <returns>The CDF value at x.</returns>
    public static double Cdf(double mean, double cv, double x)
    {
        ValidateParameters(mean, cv);
        
        if (cv == 0) return x >= mean ? 1 : 0;
        
        var stddev = cv * mean;
        return MathNet.Numerics.Distributions.Normal.CDF(mean, stddev, x);
    }
    
    /// <summary>
    /// Computes the inverse cumulative distribution function (InvCDF) of a normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the normal distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <param name="p">The probability.</param>
    /// <returns>The value x such that P(X ≤ x) = p.</returns>
    public static double InvCdf(double mean, double cv, double p)
    {
        ValidateParameters(mean, cv);
        
        var stddev = cv * mean;
        return MathNet.Numerics.Distributions.Normal.InvCDF(mean, stddev, p);
    }
    
    /// <summary>
    /// Validates the mean and coefficient of variation (CV) parameters.
    /// </summary>
    /// <param name="mean">The mean of the normal distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <param name="isMeanPossibleToBeZero">To indicate if the mean of the distribution can be zero or not.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mean or coefficient of variation are invalid.</exception>
    private static void ValidateParameters(double mean, double cv, bool isMeanPossibleToBeZero = false)
    {
        if (mean < 0)
            throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be non-negative.");
        if (!isMeanPossibleToBeZero && mean == 0)
            throw new ArgumentOutOfRangeException(nameof(mean), "Zero mean is not applicable.");
        if (cv < 0)
            throw new ArgumentOutOfRangeException(nameof(cv), "Coefficient of variation must be non-negative.");
    }
}