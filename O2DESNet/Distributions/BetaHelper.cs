namespace O2DESNet.Distributions;

/// <summary>
/// Provides utility methods for working with the Beta distribution.
/// </summary>
public static class BetaHelper
{
    /// <summary>
    /// Samples a value from a Beta distribution based on the given mean and coefficient of variation.
    /// </summary>
    /// <param name="rs">Random number generator.</param>
    /// <param name="mean">Mean of the Beta distribution.</param>
    /// <param name="cv">Coefficient of variation.</param>
    /// <returns>Sampled value from the Beta distribution.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="rs"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mean or coefficient of variation are invalid.</exception>
    public static double Sample(Random rs, double mean, double cv)
    {
        ValidateParameters(mean, cv);

        if (mean == 0 || cv == 0) return mean;

        var (alpha, beta) = ComputeAlphaBeta(mean, cv);

        return MathNet.Numerics.Distributions.Beta.Sample(rs, alpha, beta);
    }

    /// <summary>
    /// Calculates the cumulative distribution function (CDF) for a Beta distribution.
    /// </summary>
    /// <param name="mean">Mean of the Beta distribution.</param>
    /// <param name="cv">Coefficient of variation.</param>
    /// <param name="x">Value to calculate CDF for.</param>
    /// <returns>CDF value for the given x.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mean or coefficient of variation are invalid.</exception>
    public static double Cdf(double mean, double cv, double x)
    {
        ValidateParameters(mean, cv);

        var (alpha, beta) = ComputeAlphaBeta(mean, cv);

        return MathNet.Numerics.Distributions.Beta.CDF(alpha, beta, x);
    }

    /// <summary>
    /// Calculates the inverse cumulative distribution function (InvCDF) for a Beta distribution.
    /// </summary>
    /// <param name="mean">Mean of the Beta distribution.</param>
    /// <param name="cv">Coefficient of variation.</param>
    /// <param name="p">Probability value for which to calculate the InvCDF.</param>
    /// <returns>InvCDF value for the given probability p.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mean or coefficient of variation are invalid.</exception>
    public static double InvCdf(double mean, double cv, double p)
    {
        ValidateParameters(mean, cv);

        if (p is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1.");

        var (alpha, beta) = ComputeAlphaBeta(mean, cv);

        return MathNet.Numerics.Distributions.Beta.InvCDF(alpha, beta, p);
    }

    /// <summary>
    /// Validates the mean and coefficient of variation for the Beta distribution.
    /// </summary>
    /// <param name="mean">The mean value.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mean or coefficient of variation are invalid.</exception>
    private static void ValidateParameters(double mean, double cv)
    {
        if (mean is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be between 0 and 1 inclusive.");
        if (cv < 0)
            throw new ArgumentOutOfRangeException(nameof(cv), "Coefficient of variation must be non-negative.");
    }

    /// <summary>
    /// Computes the Alpha and Beta parameters for the Beta distribution based on the mean and coefficient of variation.
    /// </summary>
    /// <param name="mean">The mean of the Beta distribution.</param>
    /// <param name="cv">The coefficient of variation.</param>
    /// <returns>A tuple containing the computed Alpha and Beta parameters.</returns>
    private static (double alpha, double beta) ComputeAlphaBeta(double mean, double cv)
    {
        var stddev = cv * mean;
        var alpha = mean * mean * (1 - mean) / (stddev * stddev) - mean;
        var beta = (1 - mean) * (1 - mean) * mean / (stddev * stddev) + mean - 1;

        return (alpha, beta);
    }
}