using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    internal class BetaHelperTests
    {
        [Test]
        public void Sample_GivenValidMeanAndCv_ReturnsExpectedValue()
        {
            double mean = 0.5;
            double cv = 0.2;

            double result = BetaHelper.Sample(new Random(100), mean, cv);

            Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
        }

        [Test]
        public void Sample_MeanIsZero_ReturnsZero()
        {
            double mean = 0.0;
            double cv = 0.2;

            double result = BetaHelper.Sample(new Random(100), mean, cv);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Sample_CvIsZero_ReturnsMean()
        {
            double mean = 0.5;
            double cv = 0.0;

            double result = BetaHelper.Sample(new Random(100), mean, cv);

            Assert.That(result, Is.EqualTo(mean));
        }

        [Test]
        public void Sample_InvalidMean_ThrowsArgumentOutOfRangeException()
        {
            double mean = -0.1;
            double cv = 0.2;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.Sample(new Random(100), mean, cv));
        }

        [Test]
        public void Sample_InvalidCv_ThrowsArgumentOutOfRangeException()
        {
            double mean = 0.5;
            double cv = -0.1;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.Sample(new Random(100), mean, cv));
        }

        [Test]
        public void CDF_GivenValidMeanAndCv_ReturnsExpectedValue()
        {
            double mean = 0.5;
            double cv = 0.2;
            double x = 0.3;

            double result = BetaHelper.CDF(mean, cv, x);

            Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
        }

        [Test]
        public void CDF_InvalidMean_ThrowsArgumentOutOfRangeException()
        {
            double mean = -0.1;
            double cv = 0.2;
            double x = 0.3;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.CDF(mean, cv, x));
        }

        [Test]
        public void CDF_InvalidCv_ThrowsArgumentOutOfRangeException()
        {
            double mean = 0.5;
            double cv = -0.1;
            double x = 0.3;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.CDF(mean, cv, x));
        }

        [Test]
        public void InvCDF_GivenValidMeanAndCv_ReturnsExpectedValue()
        {
            double mean = 0.5;
            double cv = 0.2;
            double p = 0.7;

            double result = BetaHelper.InvCDF(mean, cv, p);

            Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
        }

        [Test]
        public void InvCDF_InvalidMean_ThrowsArgumentOutOfRangeException()
        {
            double mean = -0.1;
            double cv = 0.2;
            double p = 0.7;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.InvCDF(mean, cv, p));
        }

        [Test]
        public void InvCDF_InvalidCv_ThrowsArgumentOutOfRangeException()
        {
            double mean = 0.5;
            double cv = -0.1;
            double p = 0.7;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.InvCDF(mean, cv, p));
        }

        [Test]
        public void InvCDF_InvalidP_ThrowsArgumentOutOfRangeException()
        {
            double mean = 0.5;
            double cv = 0.2;
            double p = 1.5;

            Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.InvCDF(mean, cv, p));
        }

        [Test]
        public void Sample_ProducesReproducibleResults()
        {
            double mean = 0.5;
            double cv = 0.2;

            double firstSample = BetaHelper.Sample(new Random(100), mean, cv);
            double secondSample = BetaHelper.Sample(new Random(100), mean, cv);

            Assert.That(firstSample, Is.EqualTo(secondSample));
        }
    }
}
