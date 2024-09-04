using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
internal class ExponentialHelperTests
{
    private const double TolerenceAmountForEqual = 0.0000001;
    
    [Test]
    public void Sample_GivenPositiveMean_ReturnsPositiveTimeSpan()
    {
        var result = ExponentialHelper.Sample(new Random(100), TimeSpan.FromSeconds(5));

        Assert.That(result.TotalSeconds, Is.GreaterThan(0), "Sample should return a positive TimeSpan");
    }

    [Test]
    public void Sample_WhenMeanIsZeroOrNegative_ThrowsArgumentOutOfRangeException()
    {
        var mean = TimeSpan.Zero;
        
        Assert.Throws<ArgumentOutOfRangeException>(() => ExponentialHelper.Sample(new Random(100), mean));
    }

    [Test]
    public void CDF_GivenValidInputs_ReturnsExpectedValue()
    {
        const double mean = 5.0;
        const double x = 2.0;
        
        var result = ExponentialHelper.Cdf(mean, x);

        Assert.That(result, Is.EqualTo(0.3296799539643607).Within(TolerenceAmountForEqual));
    }

    [Test]
    public void InvCDF_GivenValidInputs_ReturnsExpectedValue()
    {
        const double mean = 5.0;
        const double p = 0.5;
        
        var result = ExponentialHelper.InvCdf(mean, p);

        Assert.That(result, Is.EqualTo(3.4657359027997265).Within(TolerenceAmountForEqual));
    }

    [Test]
    public void Sample_GivenSmallTimeSpan_DoesNotCausePrecisionIssues()
    {
        var mean = TimeSpan.FromMilliseconds(1);
        var result = ExponentialHelper.Sample(new Random(100), mean);

        Assert.That(result.TotalMilliseconds, Is.GreaterThan(0), "Sample should handle very small time spans without precision issues");
    }
}