﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using O2DESNet.RandomVariables.Continuous;
using System;

namespace O2DESNet.UnitTests.RandomVariableTests.Continuous
{
    [TestClass]
    public class LogNormalTests
    {
        [TestMethod]
        public void TestMeanAndVariacneConsistency()
        {
            const int numSamples = 100000;
            double mean, stdev;
            RunningStat rs = new RunningStat();
            Random defaultrs = new Random();
            LogNormal logNormal = new LogNormal();
            rs.Clear();
            mean = 2; stdev = 5;

            for (int i = 0; i < numSamples; ++i)
            {

                logNormal.Mean = mean;
                logNormal.StandardDeviation = stdev;
                rs.Push(logNormal.Sample(defaultrs));
            }
            PrintResult.CompareMeanAndVariance("logNormal", mean, stdev * stdev, rs.Mean(), rs.Variance());
        }

        [TestMethod]
        public void TestMeanAndVariacneConsistency_MuSigma()
        {
            const int numSamples = 100000;
            double mean, stdev;
            RunningStat rs = new RunningStat();
            Random defaultrs = new Random();
            LogNormal logNormal = new LogNormal();
            rs.Clear();
            mean = 2; stdev = 5;
            var muTemp = Math.Log(mean) - 0.5 * Math.Log(1 + stdev * stdev / mean / mean);
            var sigmaTemp = Math.Sqrt(Math.Log(1 + stdev * stdev / mean / mean));
            for (int i = 0; i < numSamples; ++i)
            {
                logNormal.Mu = muTemp;
                logNormal.Sigma = sigmaTemp;

                rs.Push(logNormal.Sample(defaultrs));
            }
            PrintResult.CompareMeanAndVariance("logNormal", mean, stdev * stdev, rs.Mean(), rs.Variance());
        }
    }
}
