using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    internal class ExponentialHelperTests
    {
        [Test]
        public void Sample_GivenPositiveMean_ReturnsPositiveTimeSpan()
        {
            var mean = TimeSpan.FromSeconds(5);
            var result = ExponentialHelper.Sample(new Random(100), mean, TimeUnit.Seconds);

            Assert.That(result.TotalSeconds, Is.GreaterThan(0), "Sample should return a positive TimeSpan");
        }

        [Test]
        public void Sample_WhenMeanIsZeroOrNegative_ThrowsArgumentOutOfRangeException()
        {
            var mean = TimeSpan.Zero;
            Assert.Throws<ArgumentOutOfRangeException>(() => ExponentialHelper.Sample(new Random(100), mean, TimeUnit.Seconds));
        }

        [Test]
        public void CDF_GivenValidInputs_ReturnsExpectedValue()
        {
            double mean = 5.0;
            double x = 2.0;
            double result = ExponentialHelper.CDF(mean, x);

            Assert.That(result, Is.EqualTo(0.3296799539643607).Within(0.0000001));
        }

        [Test]
        public void InvCDF_GivenValidInputs_ReturnsExpectedValue()
        {
            double mean = 5.0;
            double p = 0.5;
            double result = ExponentialHelper.InvCDF(mean, p);

            Assert.That(result, Is.EqualTo(3.4657359027997265).Within(0.0000001));
        }

        [Test]
        public void Sample_GivenSmallTimeSpan_DoesNotCausePrecisionIssues()
        {
            var mean = TimeSpan.FromMilliseconds(1);
            var result = ExponentialHelper.Sample(new Random(100), mean, TimeUnit.Seconds);

            Assert.That(result.TotalMilliseconds, Is.GreaterThan(0), "Sample should handle very small time spans without precision issues");
        }

        [Test]
        public void Sample_WithVariousTimeUnits_ReturnsConsistentResults()
        {
            var resultInSeconds = ExponentialHelper.Sample(new Random(100), TimeSpan.FromSeconds(3600), TimeUnit.Seconds).TotalSeconds;
            var resultInMinutes = ExponentialHelper.Sample(new Random(100), TimeSpan.FromMinutes(60), TimeUnit.Minutes).TotalSeconds;
            var resultInHours = ExponentialHelper.Sample(new Random(100), TimeSpan.FromHours(1), TimeUnit.Hours).TotalSeconds;
            
            // Since the random seed and mean are the same, the results should be consistent
            Assert.That(resultInSeconds, Is.EqualTo(resultInHours).Within(0.00001));
            Assert.That(resultInMinutes, Is.EqualTo(resultInHours).Within(0.00001));
            Assert.That(resultInSeconds, Is.EqualTo(resultInMinutes).Within(0.00001));
        }
    }
}