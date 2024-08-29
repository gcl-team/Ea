using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions;

[TestFixture]
public class LogNormalHelperTests
{
    [Test]
    public void Sample_ValidInput_ReturnsValue()
    {
        double result = LogNormalHelper.Sample(new Random(100), 1, 0.5);
        Assert.That(result, Is.GreaterThan(0));
    }
    
    [TestCase(0, 0.5, 0)]
    [TestCase(1, 0, 1)]
    public void Sample_ValidInput_ReturnsExpectedValue(double mean, double cv, double expectedValue)
    {
        var result = LogNormalHelper.Sample(new Random(100), mean, cv);
        
        Assert.AreEqual(expectedValue, result);
    }
    
    [TestCase(-1, 0.5)]
    [TestCase(1, -0.5)]
    public void Sample_InvalidMeanAndCv_ThrowsException(double mean, double cv)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LogNormalHelper.Sample(new Random(100), mean, cv));
    }
    
    [Test]
    public void Cdf_ValidInput_ReturnsValueBetweenZeroAndOne()
    {
        var result = LogNormalHelper.Cdf(1, 0.5, 1);
        
        Assert.That(result, Is.InRange(0, 1));
    }

    [TestCase(1, 0, 2, 1)]
    public void Cdf_ValidInput_ReturnsExpectedValue(double mean, double cv, double x, double expectedValue)
    {
        var result = LogNormalHelper.Cdf(mean, cv, x);
        
        Assert.AreEqual(expectedValue, result);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Cdf_NegativeMean_ThrowsException(double mean)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LogNormalHelper.Cdf(mean, 0.5, 1));
    }
    
    [Test]
    public void InvCdf_ValidInput_ReturnsValue()
    {
        var result = LogNormalHelper.InvCdf(1, 0.5, 0.5);
        
        Assert.That(result, Is.GreaterThan(0));
    }

    [TestCase(1, 0, 0.5, 1)]
    public void InvCdf_CvZero_ReturnsMean(double mean, double cv, double p, double expectedValue)
    {
        var result = LogNormalHelper.InvCdf(mean, cv, p);
        
        Assert.AreEqual(expectedValue, result);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void InvCDF_InvalidMean_ThrowsException(double mean)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LogNormalHelper.InvCdf(mean, 0.5, 0.5));
    }
}