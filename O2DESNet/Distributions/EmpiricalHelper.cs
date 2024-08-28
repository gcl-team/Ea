namespace O2DESNet.Distributions;

/// <summary>
/// Provides utility methods for generating random samples from an empirical distribution.
/// </summary>
public static class EmpiricalHelper
{
    /// <summary>
    /// Generates a random sample from an empirical distribution based on the given ratios.
    /// </summary>
    /// <param name="rs">The random number generator to use for sampling.</param>
    /// <param name="ratios">A collection of ratios representing the empirical distribution. Each ratio corresponds to a probability weight.</param>
    /// <returns>An integer index corresponding to the sampled value based on the given ratios.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="ratios"/> is empty.</exception>
    public static int Sample(Random rs, IEnumerable<double> ratios)
    {
        return Sample(rs, ratios, i => i);
    }

    /// <summary>
    /// Generates a random sample from an empirical distribution based on the given dictionary of values and ratios.
    /// </summary>
    /// <typeparam name="T">The type of the values in the dictionary.</typeparam>
    /// <param name="rs">The random number generator to use for sampling.</param>
    /// <param name="ratios">A dictionary where keys represent possible values and values represent their corresponding probabilities or weights.</param>
    /// <returns>The randomly sampled value from the dictionary based on the given ratios.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="ratios"/> is empty.</exception>
    public static T Sample<T>(Random rs, Dictionary<T, double> ratios) where T : notnull
    {
        return Sample(rs, ratios.Values, i => ratios.Keys.ToList()[i]);
    }

    /// <summary>
    /// Samples an item from a collection of ratios.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="rs">The random number generator.</param>
    /// <param name="ratios">An enumerable of ratios representing the relative probabilities of each index.</param>
    /// <param name="getItem">A function that returns the item based on the index.</param>
    /// <returns>The sampled item.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="ratios"/> is empty.</exception>
    private static T Sample<T>(
        Random rs,
        IEnumerable<double> ratios,
        Func<int, T> getItem)
    {
        var ratioList = ratios.ToList(); // Convert to list to use indexing
        if (ratioList.Count == 0) throw new ArgumentException("Ratios collection cannot be empty.");

        var threshold = rs.NextDouble() * ratioList.Sum();
        for (int i = 0; i < ratioList.Count; i++)
        {
            var v = ratioList[i];
            if (threshold < v) return getItem(i);
            threshold -= v;
        }

        // If the function has not returned within the loop, something went wrong.
        // This should not happen if ratios are properly normalized.
        throw new InvalidOperationException("Failed to sample from the empirical distribution. Check the ratios provided.");
    }
}