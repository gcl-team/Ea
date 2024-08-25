﻿using O2DESNet;
using O2DESNet.Distributions;
using O2DESNet.Standard;
using System;
using static O2DESNet.Distributions.ExponentialHelper;

namespace O2DESNet.Demos
{
    public class TandemQueue : Sandbox
    {
        #region Static Properties
        public double HourlyArrivalRate { get; private set; }
        public double HourlyServiceRate1 { get; private set; }
        public double HourlyServiceRate2 { get; private set; }
        public int BufferQueueSize { get { return (int)Queue2.Capacity; } }
        #endregion

        #region Dynamic Properties
        public double AvgNQueueing1 { get { return Queue1.AvgNQueueing; } }
        public double AvgNQueueing2 { get { return Queue2.AvgNQueueing; } }
        public double AvgNServing1 { get { return Server1.AvgNServing; } }        
        public double AvgNServing2 { get { return Server2.AvgNServing; } }
        public double AvgHoursInSystem { get { return HcInSystem.AverageDuration.TotalHours; } }

        private readonly IGenerator Generator;
        private readonly IQueue Queue1;
        private readonly IServer Server1;
        private readonly IQueue Queue2;
        private readonly IServer Server2;
        private readonly HourCounter HcInSystem;
        #endregion

        #region Events / Methods
        private void Arrive()
        {
            Log("Arrive");
            HcInSystem.ObserveChange(1, ClockTime);
        }

        private void Depart()
        {
            Log("Depart");
            HcInSystem.ObserveChange(-1, ClockTime);
        }
        #endregion

        /// <param name="arrRate">Hour arrival rate to the system</param>
        /// <param name="svcRate1">Hourly service rate of Server 1</param>
        /// <param name="svcRate2">Hourly service rate of Server 2</param>
        /// <param name="bufferQSize">Buffer queue (Queue 2) capacity</param>
        public TandemQueue(double arrRate, double svcRate1, double svcRate2, int bufferQSize, int seed = 0)
            : base(seed)
        {
            HourlyArrivalRate = arrRate;
            HourlyServiceRate1 = svcRate1;
            HourlyServiceRate2 = svcRate2;

            Generator = AddChild(new Generator(new Generator.Statics
            {
                InterArrivalTime = rs => ExponentialHelper.Sample(rs, TimeSpan.FromHours(1 / HourlyArrivalRate), TimeUnit.Hours)
            }, DefaultRS.Next()));

            Queue1 = AddChild(new Queue(double.PositiveInfinity, DefaultRS.Next(), id: "Queue1"));

            Server1 = AddChild(new Server(new Server.Statics
            {
                Capacity = 1,
                ServiceTime = (rs, load) => ExponentialHelper.Sample(rs, TimeSpan.FromHours(1 / HourlyServiceRate1), TimeUnit.Hours),
            }, DefaultRS.Next(), id: "Server1"));

            Queue2 = AddChild(new Queue(bufferQSize, DefaultRS.Next(), id: "Queue2"));

            Server2 = AddChild(new Server(new Server.Statics
            {
                Capacity = 1,
                ServiceTime = (rs, load) => ExponentialHelper.Sample(rs, TimeSpan.FromHours(1 / HourlyServiceRate2), TimeUnit.Hours),
            }, DefaultRS.Next(), id: "Server2"));

            Generator.OnArrive += () => Queue1.RqstEnqueue(new Load());
            Generator.OnArrive += Arrive;

            Queue1.OnEnqueued += Server1.RqstStart;
            Server1.OnStarted += Queue1.Dequeue;

            Server1.OnReadyToDepart += Queue2.RqstEnqueue;
            Queue2.OnEnqueued += Server1.Depart;

            Queue2.OnEnqueued += Server2.RqstStart;
            Server2.OnStarted += Queue2.Dequeue;

            Server2.OnReadyToDepart += Server2.Depart;
            Server2.OnReadyToDepart += load => Depart();

            HcInSystem = AddHourCounter();

            /// Initial event
            Generator.Start();
        }

        public override void Dispose() { }
    }
}
