namespace O2DESNet.Distributions;


/// <summary>
/// Provides methods for working with the Poisson distribution.
/// </summary>
public static class PoissonHelper
{
    /// <summary>
    /// Generates a random sample from a Poisson distribution.
    /// </summary>
    /// <param name="rs">Random number generator.</param>
    /// <param name="lambda">The rate parameter (λ) of the Poisson distribution. Must be non-negative.</param>
    /// <returns>A random sample from the Poisson distribution.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when lambda is negative.</exception>
    public static int Sample(Random rs, double lambda)
    {
        ValidateParameters(lambda);

        return MathNet.Numerics.Distributions.Poisson.Sample(rs, lambda);
    }

    /// <summary>
    /// Computes the cumulative distribution function (CDF) of the Poisson distribution.
    /// </summary>
    /// <param name="lambda">The rate parameter (λ) of the Poisson distribution. Must be non-negative.</param>
    /// <param name="x">The value at which to evaluate the CDF. Must be non-negative.</param>
    /// <returns>The CDF of the Poisson distribution evaluated at x.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when lambda or x is negative.</exception>
    public static double Cdf(double lambda, double x)
    {
        ValidateParameters(lambda);
        if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "x must be non-negative.");

        return MathNet.Numerics.Distributions.Poisson.CDF(lambda, x);
    }
    
    /// <summary>
    /// Validates the lambda for the Poisson distribution.
    /// </summary>
    /// <param name="lambda">The rate parameter (λ).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when lambda is invalid.</exception>
    private static void ValidateParameters(double lambda)
    {
        if (lambda <= 0)
            throw new ArgumentOutOfRangeException(nameof(lambda), "Lambda must be non-negative.");
    }
}