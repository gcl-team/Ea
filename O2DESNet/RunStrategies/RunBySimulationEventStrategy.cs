using O2DESNet.Standard;

namespace O2DESNet.RunStrategies;

public class RunBySimulationEventStrategy(SimulationEvent simulationEvent) : IRunStrategy
{
    public bool Run(Simulator simulator)
    {
        if (simulationEvent.Simulator != null && !simulationEvent.Simulator.Equals(simulator))
            return false;

        simulator.Execute(simulationEvent);
        return true;
    }
}