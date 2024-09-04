using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
internal class GammaHelperTests
{
    public enum ExpectedCondition
    {
        GreaterThanZero,
        EqualToMean
    }
    
    [TestCase(0, 0.5, ExpectedCondition.EqualToMean)]
    [TestCase(5.0, 0, ExpectedCondition.EqualToMean)]
    [TestCase(5.0, 0.2, ExpectedCondition.GreaterThanZero)]
    public void Sample_ValidParameters_ReturnsExpectedValue(double mean, double cv, ExpectedCondition expectedCondition)
    {
        var result = GammaHelper.Sample(new Random(100), mean,  cv);
        
        switch (expectedCondition)
        {
            case ExpectedCondition.EqualToMean:
                Assert.That(result, Is.EqualTo(mean));
                break;
            case ExpectedCondition.GreaterThanZero:
                Assert.That(result, Is.GreaterThan(0));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expectedCondition), expectedCondition, null);
        }
    }

    [TestCase(0, 0.5, 0)]
    [TestCase(5, 0, 5)]
    public void Sample_ValidTimeSpanMean_ReturnsExpectedValue(double mean, double cv, double expectedValue)
    {
        var result = GammaHelper.Sample(new Random(100), TimeSpan.FromSeconds(mean), 0.5);
        
        Assert.AreEqual(expectedValue, result.Seconds);
    }

    [Test]
    public void Sample_TimeSpanValidParameters_ReturnsPositiveTimeSpan()
    {
        var result = GammaHelper.Sample(new Random(100), TimeSpan.FromDays(5), 0.2);
        
        Assert.That(result.TotalDays, Is.GreaterThan(0));
    }

    [TestCase(5.0, 3.0, 0)]
    [TestCase(5.0, 5.0, 1)]
    public void Cdf_CvIsZero_XLessThanMean_ReturnsZero(double mean, double x, double expectedValue)
    {
        var result = GammaHelper.Cdf(mean, 0, x);
        
        Assert.AreEqual(expectedValue, result);
    }

    [Test]
    public void Cdf_ValidParameters_ReturnsValidProbability()
    {
        var result = GammaHelper.Cdf(5, 0.2, 4);
        
        Assert.That(result, Is.GreaterThan(0).And.LessThanOrEqualTo(1));
    }

    [Test]
    public void InvCDF_CvIsZero_ReturnsMean()
    {
        const double mean = 5.0;
        const double p = 0.5;
        
        var result = GammaHelper.InvCdf(mean, 0, p);
        
        Assert.AreEqual(mean, result);
    }

    [Test]
    public void InvCDF_ValidParameters_ReturnsPositiveValue()
    {
        var result = GammaHelper.InvCdf(5.0, 0.2, 0.5);
        
        Assert.That(result, Is.GreaterThan(0));
    }
}