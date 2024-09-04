using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
public class NormalHelperTests
{
    [TestCase(0, 1, 0)]
    [TestCase(5, 0, 5)]
    public void Sample_ValidInput_ReturnsExpectedValue(double mean, double cv, double expectedValue)
    {
        var result = NormalHelper.Sample(new Random(100), mean, cv);
        Assert.AreEqual(expectedValue, result);
    }

    [TestCase(-1, 1)]
    [TestCase(5, -1)]
    public void Sample_ThrowsException_WhenInvalidMeanAndCv(double mean, double cv)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NormalHelper.Sample(new Random(100), mean, cv));
    }

    [TestCase(0, 1, 1)]
    [TestCase(-1, 1, 1)]
    [TestCase(5, -1, 1)]
    public void CDF_ThrowsException_WhenInvalidMeanAndCv(double mean, double cv, double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NormalHelper.Cdf(mean, cv, x));
    }

    [TestCase(0, 1, 0.5)]
    [TestCase(-1, 1, 0.5)]
    [TestCase(5, -1, 0.5)]
    public void InvCDF_ThrowsException_WhenInvalidMeanAndCv(double mean, double cv, double p)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NormalHelper.InvCdf(mean, cv, p));
    }
}