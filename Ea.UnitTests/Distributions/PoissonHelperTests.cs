using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
public class PoissonTests
{
    [Test]
    public void Sample_ReturnsIntegerValue()
    {
        const double lambda = 3.5;
        
        var result = PoissonHelper.Sample(new Random(100), lambda);
        
        Assert.That(result, Is.TypeOf<int>());
    }

    [TestCase(-1)]
    [TestCase(0)]
    public void Sample_ThrowsException_WhenLambdaIsNegativeOrZero(double lambda)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PoissonHelper.Sample(new Random(100), lambda));
    }

    [Test]
    public void Cdf_ReturnsValueBetweenZeroAndOne()
    {
        const double lambda = 3.5;
        const double x = 2.0;
        
        var result = PoissonHelper.Cdf(lambda, x);
        
        Assert.That(result, Is.InRange(0.0, 1.0));
    }

    [TestCase(-1, 2)]
    [TestCase(0, 2)]
    public void Cdf_ThrowsException_WhenLambdaIsNegativeOrZero(double lambda, double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PoissonHelper.Cdf(lambda, x));
    }
}