# O²DES.NET
O²DES.NET is a framework for object-oriented discrete event simulation.

It hybrids both event-based and state-based formalism, and implement them in an object-oriented programming language. As it is event-based in the kernel, O²DES.NET is able to model the structure and behaviors of a system precisely. On top of it, the state-based formalism enables modularization and hierarchical modeling. Besides, the object-oriented paradigm abstracts the model definitions and makes them seamless to interact with analytical functionalities, regardless of their fidelity levels.

It is developed and used by C#, which facilitates flexible integration with the latest academic research in simulation analytics and enables the connection to a variety of industrial-standard modern developments from the .NET ecosystem, including mobile applications, enterprise software, Mix-Reality, and Artificial Intelligence.

## Supported .NET Version
- .NET Standard 2.0 (Recommended)
- .NET Framework 4.7.2 (Minimum Version)

## Setup Options for .NET Core 2.2/3.0 (Windows/Linux/MacOS)
- Option 1: Install [.NET Core SDK](https://dotnet.microsoft.com/download).
- Option 2: From Visual Studio Installer, enable Cross-Platform Development.

## Setup Options for .NET Framework 4.7.2 (Windows Only)
- Option 1: Install [.NET Framework 4.7.2](https://dotnet.microsoft.com/download).
- Option 2: From Visual Studio Installer, go to components and checked option to install .NET Framework 4.7.2.

# Change Log

## Version 3.6
- Improvement of HourCounter to synchronize with simulator ClockTime
https://github.com/li-haobin/O2DESNet/issues/1

## Version 3.5.1
```diff
+ Include Beta, LogNormal, Normal, Triangular distributions
```

...

## Version 2.4.1
```diff
+ Include Uniform and Exponential for the random generators.
```

## Version 2.4
```diff
+ Include a dynamic server based PathMover (Zhou et al. 2017, WSC) module for simulating traffic network.
+ Include customized Empirical and Gamma random generators, with parameters fitting simulation studies.
```
## Version 2.3.2
```diff
+ [Important Fix] All events generated are uniquely indexed. Revise the sorting mechanism the FEL (future event list) such that same time events are ordered based on the sequence they are generated. This prevents untraceable distortion of simulation results, i.e., it guarantees that the single random seed always leads to the same sample path.
- Remove conosle display if only log to file in Event. 
```
## Version 2.3.1
```diff
+ Include Histogram and Percentile methods in HourCounter, to facilitate the statistics on the distribution of the observed count.
+ Differentiate "Utilization" (when the load is being processed by the server) and "Occupation" (when the load is occupying server's capacity, i.e., both being served and stucked) for the server statistics.
```

## Version 2.3.0
```diff
+ Revise Queue and Server for higher modularity, and comply with DEVS standards.
- Temporarily remove FIFOServer, Resource, RestoreServer, and PathMover, due to unstable performance.
+ Include Synchronizer, that checks multiple conditions and triggers events when all conditions are satisfied or unsatisfied.
+ Modify "Utilization" of Server / RestoreServer by excluding those processed but not departed; instead add "Occupation" for the the inclusive counting.
+ Rename "Status" as "State" to be consistent with most simulation literature.
```

## Version 2.2.1
```diff
+ Change font-size for Path label for displaying dense path network.
+ Add X, Y, Rotate transform to Path SVG description.
```

## Verssion 2.2.0
```diff
+ In Depart() event of Server, instead of checking the first load in the Served list (and continue only if the first is departed), all loads will be screened for ToDepart condition and depart if respective condition is satisfied.
+ Implement FIFO Server, as for modelling PathMover system.
+ Add NCoccupied as dynamic property of Server / RestoreServer / FIFOServer, for easy call.
+ Add StartTimes and FinishTimes in Server, for Loads occupying the Server.
+ WriteToConsole method of Status / Component may take in clockTime:DateTime parameter, as need for determining position of vehicles in PathMover.
+ Include PathMover.
```

## Version 2.1.2
```diff
+ Allows Load to have Static properties as "Category", considering it's a special type of 2. Component that have life cycles in simulation.
+ Rename Static property of a normal Component as "Config".
+ Therefore, they coincide with Statics of a Simulation as "Scenario".
```

## Version 2.1.1
```diff
+ Simplifies structure of Component definition, by encapsulate StaticProperty based on root class, i.e., Component<TStatics> : Component.
```
## Version 2.1.0
```diff
- Remove TScenario and TStatus in Component paradigm.
+ Construct Simulator by Assembly (Component).
+ Demo for the two revision above.
```
