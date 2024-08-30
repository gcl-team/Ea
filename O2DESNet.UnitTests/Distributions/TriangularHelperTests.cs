using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions;

[TestFixture]
public class TriangularHelperTests
{
    [Test]
    public void Sample_ReturnsValueWithinBounds()
    {
        const double lower = 1.0;
        const double upper = 5.0;
        const double mode = 3.0;

        double result = TriangularHelper.Sample(new Random(100), lower, upper, mode);

        Assert.That(result, Is.InRange(lower, upper));
    }

    [TestCase(1, 5, 0)]
    [TestCase(1, 5, 6)]
    [TestCase(5, 1, 3)]
    [TestCase(5, 5, 3)]
    public void Sample_ThrowsArgumentException_WhenParametersInvalid(double lower, double upper, double mode)
    {
        Assert.Throws<ArgumentException>(() => TriangularHelper.Sample(new Random(100), lower, upper, mode));
    }

    [Test]
    public void CDF_ReturnsValueBetweenZeroAndOne()
    {
        const double lower = 1.0;
        const double upper = 5.0;
        const double mode = 3.0;
        const double x = 3.0;

        double result = TriangularHelper.Cdf(lower, upper, mode, x);

        Assert.That(result, Is.InRange(0.0, 1.0));
    }

    [TestCase(1, 5, 0, 3)]
    [TestCase(1, 5, 6, 3)]
    [TestCase(5, 1, 3, 3)]
    [TestCase(5, 5, 3, 3)]
    public void CDF_ThrowsArgumentException_WhenParametersInvalid(double lower, double upper, double mode, double x)
    {
        Assert.Throws<ArgumentException>(() => TriangularHelper.Cdf(lower, upper, mode, x));
    }

    [Test]
    public void InvCDF_ReturnsValueWithinBounds()
    {
        const double lower = 1.0;
        const double upper = 5.0;
        const double mode = 3.0;
        const double p = 0.5;

        double result = TriangularHelper.InvCdf(lower, upper, mode, p);

        Assert.That(result, Is.InRange(lower, upper));
    }
    
    [TestCase(1, 5, 0, 0.5)]
    [TestCase(1, 5, 6, 0.5)]
    [TestCase(5, 1, 3, 0.5)]
    [TestCase(5, 5, 3, 0.5)]
    public void InvCDF_ThrowsArgumentException_WhenParametersInvalid(double lower, double upper, double mode, double p)
    {
        Assert.Throws<ArgumentException>(() => TriangularHelper.InvCdf(lower, upper, mode, p));
    }

    [TestCase(1, 5, 3, -0.1)]
    [TestCase(1, 5, 3, 1.1)]
    public void InvCDF_ThrowsArgumentOutOfRangeExceptio_WhenParametersInvalid(double lower, double upper, double mode, double p)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TriangularHelper.InvCdf(lower, upper, mode, p));
    }
}