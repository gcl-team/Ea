namespace Ea.Standard;

public class SimulationEventInBatch : SimulationEventBase
{
    public required IEnumerable<SimulationEventBase> SimulationEvents { get; init; }
    
    public override void Invoke()
    {
        foreach (var simulationEvent in SimulationEvents)
        {
            Execute(simulationEvent);
        }
    }
    
}