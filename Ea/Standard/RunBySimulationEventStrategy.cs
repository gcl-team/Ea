namespace Ea.Standard;

public class RunBySimulationEventStrategy(SimulationEventBase simulationEvent) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        if (simulationEvent.Simulator != null && !simulationEvent.Simulator.Equals(simulator))
            return false;

        simulator.Execute(simulationEvent);
        return true;
    }
}