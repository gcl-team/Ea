namespace Ea.Standard;

public interface ISimulatorSandbox
{
    /// <summary>
    /// Gets the unique index assigned to this sandbox instance.
    /// </summary>
    int Index { get; }
    
    /// <summary>
    /// Gets the name of this sandbox instance.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets or sets the tag associated with this sandbox instance.
    /// </summary>
    string Tag { get; set; }
    
    /// <summary>
    /// Provides a default implementation for warming up the sandbox. 
    /// Override in derived classes to provide specific warm-up behavior.
    /// </summary>
    /// <param name="clockTime">The current simulation clock time.</param>
    void WarmedUp(DateTime clockTime);
}