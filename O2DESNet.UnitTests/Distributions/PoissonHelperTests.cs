using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    public class PoissonTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void Sample_ReturnsIntegerValue()
        {
            double lambda = 3.5;
            int result = PoissonHelper.Sample(_random, lambda);
            Assert.That(result, Is.TypeOf<int>());
        }

        [Test]
        public void Sample_ThrowsException_WhenLambdaIsNegative()
        {
            Assert.Throws<ArgumentException>(() => PoissonHelper.Sample(_random, -1));
        }

        [Test]
        public void CDF_ReturnsValueBetweenZeroAndOne()
        {
            double lambda = 3.5;
            double x = 2.0;
            double result = PoissonHelper.CDF(lambda, x);
            Assert.That(result, Is.InRange(0.0, 1.0));
        }

        [Test]
        public void CDF_ThrowsException_WhenLambdaIsNegativeOrZero()
        {
            Assert.Throws<ArgumentException>(() => PoissonHelper.CDF(-1, 2.0));
            Assert.Throws<ArgumentException>(() => PoissonHelper.CDF(0, 2.0));
        }
    }
}