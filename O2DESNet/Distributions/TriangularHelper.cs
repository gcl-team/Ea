namespace O2DESNet.Distributions;

/// <summary>
/// Provides methods for working with the Triangular distribution.
/// </summary>
public static class TriangularHelper
{
    /// <summary>
    /// Samples a random value from the triangular distribution.
    /// </summary>
    /// <param name="rs">The random number generator to use.</param>
    /// <param name="lower">The lower bound of the distribution.</param>
    /// <param name="upper">The upper bound of the distribution.</param>
    /// <param name="mode">The mode (peak) of the distribution.</param>
    /// <returns>A random value sampled from the triangular distribution.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameters do not satisfy lower <= mode <= upper.</exception>
    public static double Sample(Random rs, double lower, double upper, double mode)
    {
        ValidateParameters(lower, upper, mode);
        return MathNet.Numerics.Distributions.Triangular.Sample(rs, lower, upper, mode);
    }
    
    /// <summary>
    /// Computes the cumulative distribution function (CDF) of the triangular distribution.
    /// </summary>
    /// <param name="lower">The lower bound of the distribution.</param>
    /// <param name="upper">The upper bound of the distribution.</param>
    /// <param name="mode">The mode (peak) of the distribution.</param>
    /// <param name="x">The value at which to evaluate the CDF.</param>
    /// <returns>The probability that a random variable drawn from the triangular distribution is less than or equal to x.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameters do not satisfy lower <= mode <= upper.</exception>
    public static double Cdf(double lower, double upper, double mode, double x)
    {
        ValidateParameters(lower, upper, mode);
        return MathNet.Numerics.Distributions.Triangular.CDF(lower, upper, mode, x);
    }
    
    /// <summary>
    /// Computes the inverse cumulative distribution function (InvCDF) of the triangular distribution.
    /// </summary>
    /// <param name="lower">The lower bound of the distribution.</param>
    /// <param name="upper">The upper bound of the distribution.</param>
    /// <param name="mode">The mode (peak) of the distribution.</param>
    /// <param name="p">The probability for which to compute the corresponding value.</param>
    /// <returns>The value x such that the CDF of the triangular distribution is equal to p.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameters do not satisfy lower <= mode <= upper or when p is not in the range [0, 1].</exception>
    public static double InvCdf(double lower, double upper, double mode, double p)
    {
        ValidateParameters(lower, upper, mode);
        
        if (p is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(p), "Probability p must be between 0 and 1.");
        
        return MathNet.Numerics.Distributions.Triangular.InvCDF(lower, upper, mode, p);
    }
    
    /// <summary>
    /// Validates the parameters of the triangular distribution.
    /// </summary>
    /// <param name="lower">The lower bound of the distribution.</param>
    /// <param name="upper">The upper bound of the distribution.</param>
    /// <param name="mode">The mode (peak) of the distribution.</param>
    /// <exception cref="ArgumentException">Thrown when the parameters do not satisfy lower <= mode <= upper.</exception>
    private static void ValidateParameters(double lower, double upper, double mode)
    {
        if (lower > upper)
            throw new ArgumentException("The lower bound must be less than or equal to the upper bound.");
        if (mode < lower || mode > upper)
            throw new ArgumentException("The mode must be between the lower and upper bounds (inclusive).");
    }
}