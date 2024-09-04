using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
internal class BetaHelperTests
{
    public enum ExpectedCondition
    {
        EqualToZero,
        BetweenZeroAndOne,
        EqualToMean
    }
    
    [TestCase(0.5, 0.2, ExpectedCondition.BetweenZeroAndOne)]
    [TestCase(0.0, 0.2, ExpectedCondition.EqualToZero)]
    [TestCase(0.5, 0.0, ExpectedCondition.EqualToMean)]
    public void Sample_GivenValidMeanAndCv_ReturnsExpectedValue(double mean, double cv, ExpectedCondition expectedCondition)
    {
        var result = BetaHelper.Sample(new Random(100), mean, cv);
        
        switch (expectedCondition)
        {
            case ExpectedCondition.EqualToZero:
                Assert.That(result, Is.EqualTo(0));
                break;
            case ExpectedCondition.BetweenZeroAndOne:
                Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
                break;
            case ExpectedCondition.EqualToMean:
                Assert.That(result, Is.EqualTo(mean));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(expectedCondition), expectedCondition, null);
        }
    }

    [TestCase(-0.1, 0.2)]
    [TestCase(0.5, -0.1)]
    public void Sample_InvalidMean_ThrowsArgumentOutOfRangeException(double mean, double cv)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.Sample(new Random(100), mean, cv));
    }

    [TestCase(0.5, 0.2, 0.3)]
    public void Cdf_GivenValidMeanAndCv_ReturnsExpectedValue(double mean, double cv, double x)
    {
        var result = BetaHelper.Cdf(mean, cv, x);

        Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
    }

    [TestCase(-0.1, 0.2, 0.3)]
    [TestCase(0.5, -0.1, 0.3)]
    public void Cdf_InvalidMean_ThrowsArgumentOutOfRangeException(double mean, double cv, double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.Cdf(mean, cv, x));
    }

    [TestCase(0.5, 0.2, 0.7)]
    public void InvCdf_GivenValidMeanAndCv_ReturnsExpectedValue(double mean, double cv, double p)
    {
        var result = BetaHelper.InvCdf(mean, cv, p);

        Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
    }

    [TestCase(-0.1, 0.2, 0.7)]
    [TestCase(0.5, -0.1, 0.7)]
    [TestCase(0.5, 0.2, 1.5)]
    public void InvCdf_InvalidMean_ThrowsArgumentOutOfRangeException(double mean, double cv, double p)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BetaHelper.InvCdf(mean, cv, p));
    }

    [Test]
    public void Sample_ProducesReproducibleResults()
    {
        const double mean = 0.5;
        const double cv = 0.2;

        var firstSample = BetaHelper.Sample(new Random(100), mean, cv);
        var secondSample = BetaHelper.Sample(new Random(100), mean, cv);

        Assert.That(firstSample, Is.EqualTo(secondSample));
    }
}