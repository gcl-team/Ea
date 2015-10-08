﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O2DESNet.Demos.MM1Queue
{
    internal class Arrival : Event
    {
        internal Customer Customer { get; private set; }
        internal Arrival(Simulator sim, Customer customer) : base(sim) { Customer = customer; }
        public override void Invoke()
        {
            if (_sim.Status.Serving == null) new StartService(_sim, Customer).Invoke();
            else _sim.Status.WaitingQueue.Enqueue(Customer);
            _sim.ScheduleEvent(new Arrival(_sim, new Customer()), _sim.Scenario.Generate_InterArrivalTime(_sim.RS));
        }
    }
}
