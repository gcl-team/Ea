namespace O2DESNet.Distributions;

/// <summary>
/// Provides helper methods for working with the Gamma distribution.
/// </summary>
public static class GammaHelper
{
    /// <summary>
    /// Generates a random sample from a Gamma distribution using the provided mean and coefficient of variation (cv).
    /// </summary>
    /// <param name="rs">The random number generator used to produce the sample.</param>
    /// <param name="mean">The mean of the Gamma distribution.</param>
    /// <param name="cv">The coefficient of variation (standard deviation divided by mean).</param>
    /// <returns>A random sample from the Gamma distribution.</returns>
    public static double Sample(Random rs, double mean, double cv)
    {
        ValidateParameters(mean, cv, true);
        
        if (mean == 0 || cv == 0) return mean;
        
        var (k, lambda) = ComputeKLambda(mean, cv);
        
        return MathNet.Numerics.Distributions.Gamma.Sample(rs, k, lambda);
    }

    /// <summary>
    /// Calculates the cumulative distribution function (CDF) of the Gamma distribution at a specified value.
    /// </summary>
    /// <param name="mean">The mean of the Gamma distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <returns>The cumulative probability up to the specified value x.</returns>
    public static double Cdf(double mean, double cv, double x)
    {
        ValidateParameters(mean, cv);
        
        if (cv == 0) return x >= mean ? 1 : 0;
        
        var (k, lambda) = ComputeKLambda(mean, cv);
        
        return MathNet.Numerics.Distributions.Gamma.CDF(k, lambda, x);
    }

    /// <summary>
    /// Calculates the inverse cumulative distribution function (InvCDF) of the Gamma distribution for a specified probability.
    /// </summary>
    /// <param name="mean">The mean of the Gamma distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <param name="p">The probability for which to calculate the inverse CDF.</param>
    /// <returns>The value corresponding to the specified cumulative probability p.</returns>
    public static double InvCdf(double mean, double cv, double p)
    {
        ValidateParameters(mean, cv);
        
        if (cv == 0) return mean;
        
        var (k, lambda) = ComputeKLambda(mean, cv);
        
        return MathNet.Numerics.Distributions.Gamma.InvCDF(k, lambda, p);
    }

    /// <summary>
    /// Generates a random sample from a Gamma distribution where the mean is represented as a TimeSpan.
    /// </summary>
    /// <param name="rs">The random number generator used to produce the sample.</param>
    /// <param name="mean">The mean of the Gamma distribution expressed as a TimeSpan.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <returns>A random sample from the Gamma distribution as a TimeSpan.</returns>
    public static TimeSpan Sample(Random rs, TimeSpan mean, double cv)
    {
        return TimeSpan.FromSeconds(Sample(rs, mean.TotalSeconds, cv));
    }
    
    /// <summary>
    /// Validates the mean and coefficient of variation for the Gamma distribution.
    /// </summary>
    /// <param name="mean">The mean value.</param>
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
    
    /// <summary>
    /// Compute the shape (k) and rate (lambda) parameters for the Gamma distribution
    /// </summary>
    /// <param name="mean">The mean of the Gamma distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <returns>A tuple containing the computed k and lambda parameters.</returns>
    private static (double k, double lambda) ComputeKLambda(double mean, double cv)
    {
        var k = 1 / cv / cv;
        var lambda = k / mean;

        return (k, lambda);
    }
}