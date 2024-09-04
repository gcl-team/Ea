using NUnit.Framework;
using Ea.Distributions;

namespace Ea.UnitTests.Distributions;

[TestFixture]
public class UniformHelperTests
{
    public static IEnumerable<object[]> SuccessfulSampleTestCases
    {
        get
        {
            yield return new object[] { 1, 10 };
            yield return new object[] { TimeSpan.FromMinutes(10).TotalSeconds, TimeSpan.FromMinutes(60).TotalSeconds };
        }
    }
    
    public static IEnumerable<object[]> FailedSampleTestCases
    {
        get
        {
            yield return new object[] { 10, 1 };
            yield return new object[] { TimeSpan.FromMinutes(60).TotalSeconds, TimeSpan.FromMinutes(10).TotalSeconds };
        }
    }
    
    [TestCaseSource(nameof(SuccessfulSampleTestCases))]
    public void Sample_ReturnsValueWithinBounds(double lower, double upper)
    {
        var result = UniformHelper.Sample(new Random(100), lower, upper);

        Assert.That(result, Is.InRange(lower, upper));
    }

    [TestCaseSource(nameof(FailedSampleTestCases))]
    public void Sample_Double_ThrowsException_WhenLowerBoundGreaterThanUpperBound(double lower, double upper)
    {
        Assert.Throws<ArgumentException>(() => UniformHelper.Sample(new Random(100), lower, upper));
    }

    [Test]
    public void Sample_Generic_ReturnsElementFromCandidates()
    {
        var candidates = new List<int> { 1, 2, 3, 4, 5 };

        var result = UniformHelper.Sample(new Random(100), candidates);

        Assert.That(candidates.Contains(result), Is.True);
    }

    [Test]
    public void Sample_Generic_ThrowsException_WhenCandidatesEmpty()
    {
        var candidates = new List<int>();

        Assert.Throws<ArgumentException>(() => UniformHelper.Sample(new Random(100), candidates));
    }

    [Test]
    public void Sample_Generic_ReturnsNull_WhenCandidatesEmptyAndTypeIsReference()
    {
        var candidates = new List<string?>() { null };

        Assert.Throws<ArgumentException>(() => UniformHelper.Sample(new Random(100), candidates));
    }
}