using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    public class TriangularHelperTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void Sample_ReturnsValueWithinBounds()
        {
            double lower = 1.0;
            double upper = 5.0;
            double mode = 3.0;

            double result = TriangularHelper.Sample(_random, lower, upper, mode);

            Assert.That(result, Is.InRange(lower, upper));
        }

        [Test]
        public void Sample_ThrowsException_WhenModeIsOutOfRange()
        {
            double lower = 1.0;
            double upper = 5.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.Sample(_random, lower, upper, 0.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.Sample(_random, lower, upper, 6.0));
        }

        [Test]
        public void Sample_ThrowsException_WhenLowerIsGreaterThanOrEqualToUpper()
        {
            double mode = 3.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.Sample(_random, 5.0, 1.0, mode));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.Sample(_random, 5.0, 5.0, mode));
        }

        [Test]
        public void CDF_ReturnsValueBetweenZeroAndOne()
        {
            double lower = 1.0;
            double upper = 5.0;
            double mode = 3.0;
            double x = 3.0;

            double result = TriangularHelper.CDF(lower, upper, mode, x);

            Assert.That(result, Is.InRange(0.0, 1.0));
        }

        [Test]
        public void CDF_ThrowsException_WhenModeIsOutOfRange()
        {
            double lower = 1.0;
            double upper = 5.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.CDF(lower, upper, 0.0, 3.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.CDF(lower, upper, 6.0, 3.0));
        }

        [Test]
        public void CDF_ThrowsException_WhenLowerIsGreaterThanOrEqualToUpper()
        {
            double mode = 3.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.CDF(5.0, 1.0, mode, 3.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.CDF(5.0, 5.0, mode, 3.0));
        }

        [Test]
        public void InvCDF_ReturnsValueWithinBounds()
        {
            double lower = 1.0;
            double upper = 5.0;
            double mode = 3.0;
            double p = 0.5;

            double result = TriangularHelper.InvCDF(lower, upper, mode, p);

            Assert.That(result, Is.InRange(lower, upper));
        }

        [Test]
        public void InvCDF_ThrowsException_WhenProbabilityIsOutOfRange()
        {
            double lower = 1.0;
            double upper = 5.0;
            double mode = 3.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(lower, upper, mode, -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(lower, upper, mode, 1.1));
        }

        [Test]
        public void InvCDF_ThrowsException_WhenModeIsOutOfRange()
        {
            double lower = 1.0;
            double upper = 5.0;
            double p = 0.5;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(lower, upper, 0.0, p));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(lower, upper, 6.0, p));
        }

        [Test]
        public void InvCDF_ThrowsException_WhenLowerIsGreaterThanOrEqualToUpper()
        {
            double mode = 3.0;
            double p = 0.5;

            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(5.0, 1.0, mode, p));
            Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCDF(5.0, 5.0, mode, p));
        }
    }
}