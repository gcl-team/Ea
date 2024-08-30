namespace O2DESNet.Distributions;

/// <summary>
/// Provides methods for working with the Uniform distribution.
/// </summary>
public static class UniformHelper
{
    /// <summary>
    /// Generates a random sample from a uniform distribution between the specified bounds.
    /// </summary>
    /// <param name="rs">A random number generator.</param>
    /// <param name="lower">The lower bound of the uniform distribution.</param>
    /// <param name="upper">The upper bound of the uniform distribution.</param>
    /// <returns>A random double within the specified bounds.</returns>
    /// <exception cref="ArgumentException">Thrown if lowerBound is greater than upperBound.</exception>
    public static double Sample(Random rs, double lower, double upper)
    {
        ValidateParameters(lower, upper);
        
        return lower + (upper - lower) * rs.NextDouble();
    }

    /// <summary>
    /// Generates a random TimeSpan sample from a uniform distribution between the specified bounds.
    /// </summary>
    /// <param name="rs">A random number generator.</param>
    /// <param name="lower">The lower bound of the TimeSpan uniform distribution.</param>
    /// <param name="upper">The upper bound of the TimeSpan uniform distribution.</param>
    /// <returns>A random TimeSpan within the specified bounds.</returns>
    public static TimeSpan Sample(Random rs, TimeSpan lower, TimeSpan upper)
    {
        ValidateParameters(lower.TotalSeconds, upper.TotalSeconds);
        
        return TimeSpan.FromSeconds(Sample(rs, lower.TotalSeconds, upper.TotalSeconds));
    }

    /// <summary>
    /// Randomly selects an element from a list of candidates.
    /// </summary>
    /// <typeparam name="T">The type of elements in the candidates list.</typeparam>
    /// <param name="rs">A random number generator.</param>
    /// <param name="candidates">The list of candidate elements.</param>
    /// <returns>A randomly selected element from the candidates list.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="candidates"/> is empty or contains null elements.</exception>
    public static T Sample<T>(Random rs, IEnumerable<T> candidates)
    {
        var candidateList = candidates.ToList(); // Materialize the collection to avoid multiple enumeration
        
        if (candidateList.Count == 0)
            throw new ArgumentException("Candidates collection cannot be empty.");

        var selectedItem = candidateList[rs.Next(candidateList.Count)];

        if (selectedItem == null)
            throw new ArgumentException("The candidates collection contains null values, which are not allowed.");

        return selectedItem;
    }
    
    /// <summary>
    /// Validates the parameters of the uniform distribution.
    /// </summary>
    /// <param name="lower">The lower bound of the distribution.</param>
    /// <param name="upper">The upper bound of the distribution.</param>
    /// <param name="mode">The mode (peak) of the distribution.</param>
    /// <exception cref="ArgumentException">Thrown when the parameters do not satisfy lower <= mode <= upper.</exception>
    private static void ValidateParameters(double lower, double upper)
    {
        if (lower > upper)
            throw new ArgumentException("The lower bound must be less than or equal to the upper bound.");
    }
}