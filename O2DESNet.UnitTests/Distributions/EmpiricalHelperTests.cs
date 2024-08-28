using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions;

[TestFixture]
internal class EmpiricalHelperTests
{
    public static IEnumerable<object[]> ThrowsExceptionTestCases()
    {
        yield return [new List<double>(), typeof(ArgumentException)];
        yield return [new List<double> { 0.0, 0.0, 0.0 }, typeof(InvalidOperationException)];
    }
    
    [Test]
    public void Sample_GivenValidRatios_ReturnsExpectedIndex()
    {
        var ratios = new List<double> { 0.1, 0.3, 0.6 };
        var result = EmpiricalHelper.Sample(new Random(100), ratios);

        Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThan(ratios.Count));
    }

    [TestCaseSource(nameof(ThrowsExceptionTestCases))]
    public void Sample_WithEmptyRatios_ThrowsArgumentException(List<double> ratios, Type expectedException)
    {
        Assert.Throws(expectedException, () => EmpiricalHelper.Sample(new Random(100), ratios));
    }
    
    [Test]
    public void Sample_GivenValidDictionary_ReturnsExpectedKey()
    {
        var ratios = new Dictionary<string, double>
        {
            { "A", 0.2 },
            { "B", 0.5 },
            { "C", 0.3 }
        };

        var result = EmpiricalHelper.Sample(new Random(100), ratios);

        Assert.That(ratios.ContainsKey(result));
    }

    [Test]
    public void Sample_WithEmptyDictionary_ThrowsArgumentException()
    {
        var ratios = new Dictionary<string, double>();

        Assert.Throws<ArgumentException>(() => EmpiricalHelper.Sample(new Random(100), ratios));
    }

    [Test]
    public void Sample_WithAllZeroDictionaryValues_ThrowsInvalidOperationException()
    {
        var ratios = new Dictionary<string, double>
        {
            { "A", 0.0 },
            { "B", 0.0 },
            { "C", 0.0 }
        };

        Assert.Throws<InvalidOperationException>(() => EmpiricalHelper.Sample(new Random(100), ratios));
    }

    [Test]
    public void Sample_ProducesReproducibleResults()
    {
        var ratios = new List<double> { 0.2, 0.5, 0.3 };
        var firstSample = EmpiricalHelper.Sample(new Random(100), ratios);
        var secondSample = EmpiricalHelper.Sample(new Random(100), ratios);

        Assert.That(firstSample, Is.EqualTo(secondSample));
    }
}