﻿using O2DESNet.Explorers;
using O2DESNet.Replicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace O2DESNet.Demos.Workshop
{
    class Program
    {
        static void Main(string[] args)
        {
            var randomSearch = new RandomSearch<Scenario, Status, Simulator>(
                decisionSpace: new DecisionSpace(new double[] { 2, 2, 2, 2, 2 }, new double[] { 10, 10, 10, 10, 10 }),
                constrScenario: decision => Scenario.GetExample_Xu2015(decision.Select(d => (int)d).ToArray()),
                constrStatus: (scenario, seed) => new Status(scenario, seed),
                constrSimulator: status => new Simulator(status),
                terminate: status => status.TimeSeries_ProductHoursInSystem.Count >= 100,
                objective: status => status.TimeSeries_ProductHoursInSystem.Average(),
                inDifferentZone: 0.1,
                discrete: true
                );
            while (true)
            {
                Console.Clear();
                randomSearch.Iterate(10, 100);
                randomSearch.Replicator.Display();
                Console.ReadKey();
            }
        }

        static void Test_Replicator(string[] args)
        {
            var minSelector =
                new OCBA<Scenario, Status, Simulator>(
                //new MinSelector<Scenario, Status, Simulator>(
                scenarios: new Scenario[] {
                    Scenario.GetExample_Xu2015(7, 3, 7, 9, 6),
                    Scenario.GetExample_Xu2015(3, 6, 5, 5, 7),
                    Scenario.GetExample_Xu2015(7, 3, 2, 6, 5),
                },
                constrStatus: (scenario, seed) => new Status(scenario, seed),
                constrSimulator: status => new Simulator(status),
                terminate: status => status.TimeSeries_ProductHoursInSystem.Count >= 50,
                objective: status => status.TimeSeries_ProductHoursInSystem.Average());

            // initialize
            minSelector.Alloc(100);

            for (int i = 0; i < 50; i++)
            {
                Console.Write("{0}\t", minSelector.PCS);
                //minSelector.EqualAlloc(90);
                minSelector.Alloc(90);
                //Console.ReadKey();
                foreach (var sc in minSelector.Scenarios) Console.Write("{0},", minSelector.Objectives[sc].Count);
                Console.WriteLine();
            }

            minSelector.Add(Scenario.GetExample_Xu2015(6, 5, 7, 9, 8));
            for (int i = 0; i < 50; i++)
            {
                Console.Write("{0}\t", minSelector.PCS);
                minSelector.Alloc(90);
                foreach (var sc in minSelector.Scenarios) Console.Write("{0},", minSelector.Objectives[sc].Count);
                Console.WriteLine();
            }
        }

        static void Test_Simulator()
        {
            int seed = 0;

            var sim = new Simulator(new Status(Scenario.GetExample_PedrielliZhu2015(2, 5, 4, 3, 6))
            {
                Seed = seed,
                //Display = true,
                LogFile = string.Format("workshop_log_{0}.txt", seed),
            });

            //var sim = new Simulator(new Status(Scenario.GetExample_Xu2015(6, 5, 7, 9, 8))
            //{
            //    Seed = seed,
            //    Display = true,
            //    LogFile = string.Format("workshop_log_{0}.txt", seed),
            //});
            sim.Run(TimeSpan.FromDays(30));
        }
    }
}
