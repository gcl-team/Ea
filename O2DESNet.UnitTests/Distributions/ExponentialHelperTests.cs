using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions;

[TestFixture]
internal class ExponentialHelperTests
{
    private const double TolerenceAmountForEqual = 0.0000001;
    
    [Test]
    public void Sample_GivenPositiveMean_ReturnsPositiveTimeSpan()
    {
        var result = ExponentialHelper.Sample(new Random(100), TimeSpan.FromSeconds(5), TimeUnit.Seconds);

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
        Assert.That(resultInSeconds, Is.EqualTo(resultInHours).Within(TolerenceAmountForEqual));
        Assert.That(resultInMinutes, Is.EqualTo(resultInHours).Within(TolerenceAmountForEqual));
        Assert.That(resultInSeconds, Is.EqualTo(resultInMinutes).Within(TolerenceAmountForEqual));
    }
}