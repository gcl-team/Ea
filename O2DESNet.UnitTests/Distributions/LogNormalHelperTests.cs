using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    public class LogNormalHelperTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void Sample_NegativeMean_ThrowsException()
        {
            Assert.Throws<Exception>(() => LogNormalHelper.Sample(_random, -1, 0.5));
        }

        [Test]
        public void Sample_NegativeCv_ThrowsException()
        {
            Assert.Throws<Exception>(() => LogNormalHelper.Sample(_random, 1, -0.5));
        }

        [Test]
        public void Sample_MeanZero_ReturnsZero()
        {
            double result = LogNormalHelper.Sample(_random, 0, 0.5);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Sample_CvZero_ReturnsMean()
        {
            double mean = 1;
            double result = LogNormalHelper.Sample(_random, mean, 0);
            Assert.AreEqual(mean, result);
        }

        [Test]
        public void Sample_ValidInput_ReturnsValue()
        {
            double result = LogNormalHelper.Sample(_random, 1, 0.5);
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void CDF_MeanZero_ReturnsOneIfXGreaterOrEqualToMean()
        {
            double mean = 0;
            double cv = 0;
            double x = 1;
            double result = LogNormalHelper.CDF(mean, cv, x);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CDF_NegativeMean_ThrowsException()
        {
            Assert.Throws<Exception>(() => LogNormalHelper.CDF(-1, 0.5, 1));
        }

        [Test]
        public void CDF_CvZero_ReturnsCorrectValue()
        {
            double mean = 1;
            double cv = 0;
            double x = 2;
            double result = LogNormalHelper.CDF(mean, cv, x);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CDF_ValidInput_ReturnsValueBetweenZeroAndOne()
        {
            double result = LogNormalHelper.CDF(1, 0.5, 1);
            Assert.That(result, Is.InRange(0, 1));
        }

        [Test]
        public void InvCDF_MeanZero_ReturnsMean()
        {
            double mean = 1;
            double result = LogNormalHelper.InvCDF(mean, 0, 0.5);
            Assert.AreEqual(mean, result);
        }

        [Test]
        public void InvCDF_NegativeMean_ThrowsException()
        {
            Assert.Throws<Exception>(() => LogNormalHelper.InvCDF(-1, 0.5, 0.5));
        }

        [Test]
        public void InvCDF_CvZero_ReturnsMean()
        {
            double mean = 1;
            double result = LogNormalHelper.InvCDF(mean, 0, 0.5);
            Assert.AreEqual(mean, result);
        }

        [Test]
        public void InvCDF_ValidInput_ReturnsValue()
        {
            double result = LogNormalHelper.InvCDF(1, 0.5, 0.5);
            Assert.That(result, Is.GreaterThan(0));
        }
    }
}