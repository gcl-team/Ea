namespace O2DESNet.Distributions;

/// <summary>
/// Provides methods for sampling from a log-normal distribution.
/// </summary>
public static class LogNormalHelper
{
    /// <summary>
    /// Samples a value from a log-normal distribution given the mean and coefficient of variation (CV).
    /// </summary>
    /// <param name="rs">A random number generator.</param>
    /// <param name="mean">The mean of the log-normal distribution.</param>
    /// <param name="cv">The coefficient of variation (CV).</param>
    /// <returns>A sampled value from the log-normal distribution.</returns>
    public static double Sample(Random rs, double mean, double cv)
    {
        ValidateParameters(mean, cv, true);
        
        if (mean == 0 || cv == 0) return mean;

        var variance = Math.Pow(cv * mean, 2);
        var mu = Math.Log(mean * mean / Math.Sqrt(variance + mean * mean));
        var sigma = Math.Sqrt(Math.Log(variance / (mean * mean) + 1));

        return MathNet.Numerics.Distributions.LogNormal.Sample(rs, mu, sigma);
    }

    /// <summary>
    /// Calculates the cumulative distribution function (CDF) of a log-normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the log-normal distribution.</param>
    /// <param name="cv">The coefficient of variation (CV).</param>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <returns>The probability that a value sampled from the distribution is less than or equal to x.</returns>
    public static double Cdf(double mean, double cv, double x)
    {
        ValidateParameters(mean, cv);
        
        if (cv == 0) return x >= mean ? 1 : 0;

        var variance = Math.Pow(cv * mean, 2);
        var mu = Math.Log(mean * mean / Math.Sqrt(variance + mean * mean));
        var sigma = Math.Sqrt(Math.Log(variance / (mean * mean) + 1));

        return MathNet.Numerics.Distributions.LogNormal.CDF(mu, sigma, x);
    }

    /// <summary>
    /// Calculates the inverse cumulative distribution function (InvCDF) of a log-normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the log-normal distribution.</param>
    /// <param name="cv">The coefficient of variation (CV).</param>
    /// <param name="p">The probability value for which to find the corresponding x value.</param>
    /// <returns>The value x for which the CDF of the distribution equals p.</returns>
    public static double InvCdf(double mean, double cv, double p)
    {
        ValidateParameters(mean, cv);
        
        if (cv == 0) return mean;

        var variance = Math.Pow(cv * mean, 2);
        var mu = Math.Log(mean * mean / Math.Sqrt(variance + mean * mean));
        var sigma = Math.Sqrt(Math.Log(variance / (mean * mean) + 1));

        return MathNet.Numerics.Distributions.LogNormal.InvCDF(mu, sigma, p);
    }

    /// <summary>
    /// Validates the mean and coefficient of variation for the log-normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="cv">The coefficient of variation of the distribution.</param>
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