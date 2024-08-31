namespace O2DESNet.Standard;

public interface ISimulatorSandbox
{
    int Id { get; }
    /// <summary>
    /// Name of the module type
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Tag for the module instance
    /// </summary>
    string Tag { get; set; }
    void WarmedUp(DateTime clockTime);
}