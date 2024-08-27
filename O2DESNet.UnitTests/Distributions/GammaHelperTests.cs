using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions;

[TestFixture]
public class GammaHelperTests
{
    private Random _random;

    [SetUp]
    public void SetUp()
    {
        _random = new Random();
    }

    [Test]
    public void Sample_MeanIsZero_ReturnsZero()
    {
        var result = GammaHelper.Sample(_random, 0, 0.5);
        Assert.AreEqual(0, result);
    }

    [Test]
    public void Sample_CvIsZero_ReturnsMean()
    {
        double mean = 5.0;
        var result = GammaHelper.Sample(_random, mean, 0);
        Assert.AreEqual(mean, result);
    }

    [Test]
    public void Sample_ValidParameters_ReturnsValue()
    {
        var result = GammaHelper.Sample(_random, 5.0, 0.2);
        Assert.That(result, Is.GreaterThan(0));
    }

    [Test]
    public void Sample_TimeSpanMeanIsZero_ReturnsZeroTimeSpan()
    {
        var result = GammaHelper.Sample(_random, TimeSpan.Zero, 0.5);
        Assert.AreEqual(TimeSpan.Zero, result);
    }

    [Test]
    public void Sample_TimeSpanCvIsZero_ReturnsMean()
    {
        TimeSpan mean = TimeSpan.FromDays(5);
        var result = GammaHelper.Sample(_random, mean, 0);
        Assert.AreEqual(mean, result);
    }

    [Test]
    public void Sample_TimeSpanValidParameters_ReturnsPositiveTimeSpan()
    {
        var result = GammaHelper.Sample(_random, TimeSpan.FromDays(5), 0.2);
        Assert.That(result.TotalDays, Is.GreaterThan(0));
    }

    [Test]
    public void CDF_CvIsZero_XLessThanMean_ReturnsZero()
    {
        double mean = 5.0;
        double x = 3.0;
        var result = GammaHelper.CDF(mean, 0, x);
        Assert.AreEqual(0, result);
    }

    [Test]
    public void CDF_CvIsZero_XGreaterThanOrEqualMean_ReturnsOne()
    {
        double mean = 5.0;
        double x = 5.0;
        var result = GammaHelper.CDF(mean, 0, x);
        Assert.AreEqual(1, result);
    }

    [Test]
    public void CDF_ValidParameters_ReturnsValidProbability()
    {
        var result = GammaHelper.CDF(5.0, 0.2, 4.0);
        Assert.That(result, Is.GreaterThan(0).And.LessThanOrEqualTo(1));
    }

    [Test]
    public void InvCDF_CvIsZero_ReturnsMean()
    {
        double mean = 5.0;
        double p = 0.5;
        var result = GammaHelper.InvCDF(mean, 0, p);
        Assert.AreEqual(mean, result);
    }

    [Test]
    public void InvCDF_ValidParameters_ReturnsPositiveValue()
    {
        var result = GammaHelper.InvCDF(5.0, 0.2, 0.5);
        Assert.That(result, Is.GreaterThan(0));
    }
}