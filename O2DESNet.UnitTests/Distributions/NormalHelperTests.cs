using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    public class NormalHelperTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void Sample_ReturnsZero_WhenMeanIsZero()
        {
            var result = NormalHelper.Sample(_random, 0, 1);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Sample_ReturnsMean_WhenCVIsZero()
        {
            var mean = 5;
            var result = NormalHelper.Sample(_random, mean, 0);
            Assert.AreEqual(mean, result);
        }

        [Test]
        public void Sample_ThrowsException_WhenMeanIsNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.Sample(_random, -1, 1), "Negative mean not applicable");
        }

        [Test]
        public void Sample_ThrowsException_WhenCVIsNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.Sample(_random, 5, -1), "Negative coefficient of variation not applicable for normal distribution");
        }

        [Test]
        public void CDF_ThrowsException_WhenCVIsZero()
        {
            Assert.Throws<Exception>(() => NormalHelper.CDF(1, 0, 1), "Zero or negative coefficient of variation not applicable for normal distribution");
        }

        [Test]
        public void CDF_ThrowsException_WhenMeanIsZeroOrNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.CDF(0, 1, 1), "Zero or negative mean not applicable");
            Assert.Throws<Exception>(() => NormalHelper.CDF(-1, 1, 1), "Zero or negative mean not applicable");
        }

        [Test]
        public void CDF_ThrowsException_WhenCVIsZeroOrNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.CDF(5, 0, 1), "Zero or negative coefficient of variation not applicable for normal distribution");
            Assert.Throws<Exception>(() => NormalHelper.CDF(5, -1, 1), "Zero or negative coefficient of variation not applicable for normal distribution");
        }

        [Test]
        public void InvCDF_ThrowsException_WhenMeanIsZeroOrNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.InvCDF(0, 1, 0.5), "Zero or negative mean not applicable");
            Assert.Throws<Exception>(() => NormalHelper.InvCDF(-1, 1, 0.5), "Zero or negative mean not applicable");
        }

        [Test]
        public void InvCDF_ThrowsException_WhenCVIsZeroOrNegative()
        {
            Assert.Throws<Exception>(() => NormalHelper.InvCDF(5, 0, 0.5), "Zero or negative coefficient of variation not applicable for normal distribution");
            Assert.Throws<Exception>(() => NormalHelper.InvCDF(5, -1, 0.5), "Zero or negative coefficient of variation not applicable for normal distribution");
        }
    }
}