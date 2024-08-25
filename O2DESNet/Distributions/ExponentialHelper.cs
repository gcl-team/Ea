using System;

namespace O2DESNet.Distributions
{
    /// <summary>
    /// Provides methods for sampling from an exponential distribution.
    /// </summary>
    public static class ExponentialHelper
    {
        public enum TimeUnit
        {
            Days,
            Hours,
            Minutes,
            Seconds,
            Milliseconds
        }

        /// <summary>
        /// Samples a value from an exponential distribution given a mean.
        /// </summary>
        /// <param name="rs">The random number generator.</param>
        /// <param name="mean">The mean of the exponential distribution.</param>
        /// <returns>The sampled value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mean is less than or equal to zero.</exception>
        public static double Sample(Random rs, double mean)
        {
            ValidateMean(mean);

            return MathNet.Numerics.Distributions.Exponential.Sample(rs, 1 / mean);
        }

        /// <summary>
        /// Samples a time span value from an exponential distribution given a mean time span.
        /// </summary>
        /// <param name="rs">The random number generator.</param>
        /// <param name="mean">The mean time span of the exponential distribution.</param>
        /// <param name="timeUnit">The unit of the TimeSpan to return.</param>
        /// <returns>The sampled time span in the specified unit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mean time span is less than or equal to zero or if an invalid unit is provided..</exception>
        public static TimeSpan Sample(Random rs, TimeSpan mean, TimeUnit timeUnit)
        {
            return timeUnit switch
            {
                TimeUnit.Days => TimeSpan.FromDays(Sample(rs, mean.TotalDays)),
                TimeUnit.Hours => TimeSpan.FromHours(Sample(rs, mean.TotalHours)),
                TimeUnit.Minutes => TimeSpan.FromMinutes(Sample(rs, mean.TotalMinutes)),
                TimeUnit.Seconds => TimeSpan.FromSeconds(Sample(rs, mean.TotalSeconds)),
                TimeUnit.Milliseconds => TimeSpan.FromMilliseconds(Sample(rs, mean.TotalMilliseconds)),
                _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Unsupported time unit.")
            };
        }

        /// <summary>
        /// Computes the cumulative distribution function (CDF) for a given value.
        /// </summary>
        /// <param name="mean">The mean of the exponential distribution.</param>
        /// <param name="x">The value for which the CDF is computed.</param>
        /// <returns>The CDF value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mean is less than or equal to zero.</exception>
        public static double CDF(double mean, double x)
        {
            ValidateMean(mean);

            return MathNet.Numerics.Distributions.Exponential.CDF(1 / mean, x);
        }

        /// <summary>
        /// Computes the inverse cumulative distribution function (InvCDF) for a given probability.
        /// </summary>
        /// <param name="mean">The mean of the exponential distribution.</param>
        /// <param name="p">The probability for which the InvCDF is computed.</param>
        /// <returns>The value corresponding to the given probability.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mean is less than or equal to zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the probability is not between 0 and 1.</exception>
        public static double InvCDF(double mean, double p)
        {
            ValidateMean(mean);
            if (p < 0 || p > 1) throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1.");

            return MathNet.Numerics.Distributions.Exponential.InvCDF(1 / mean, p);
        }

        /// <summary>
        /// Validates that the mean is greater than zero.
        /// </summary>
        /// <param name="mean">The mean value to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the mean is less than or equal to zero.</exception>
        private static void ValidateMean(double mean)
        {
            double epsilon = 1e-10; // A small value to avoid division by zero

            if (mean <= epsilon) throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be greater than zero.");
        }
    }
}
