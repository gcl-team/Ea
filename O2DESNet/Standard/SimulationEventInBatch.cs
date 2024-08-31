namespace O2DESNet.Standard;

public class SimulationEventInBatch : SimulationEvent
{
    public required IEnumerable<SimulationEvent> Events { get; init; }
    
    public override void Invoke()
    {
        foreach (var simulationEvent in Events)
        {
            Execute(simulationEvent);
        }
    }
    
}