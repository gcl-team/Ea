using NUnit.Framework;
using O2DESNet.Distributions;

namespace O2DESNet.UnitTests.Distributions
{
    [TestFixture]
    public class UniformHelperTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
        }

        [Test]
        public void Sample_Double_ReturnsValueWithinBounds()
        {
            double lowerbound = 1.0;
            double upperbound = 10.0;

            double result = UniformHelper.Sample(_random, lowerbound, upperbound);

            Assert.That(result, Is.InRange(lowerbound, upperbound));
        }

        [Test]
        public void Sample_Double_ThrowsException_WhenLowerBoundGreaterThanUpperBound()
        {
            double lowerbound = 10.0;
            double upperbound = 1.0;

            Assert.Throws<ArgumentOutOfRangeException>(() => UniformHelper.Sample(_random, lowerbound, upperbound));
        }

        [Test]
        public void Sample_TimeSpan_ReturnsValueWithinBounds()
        {
            TimeSpan lowerbound = TimeSpan.FromMinutes(10);
            TimeSpan upperbound = TimeSpan.FromMinutes(60);

            TimeSpan result = UniformHelper.Sample(_random, lowerbound, upperbound);

            Assert.That(result, Is.GreaterThanOrEqualTo(lowerbound).And.LessThanOrEqualTo(upperbound));
        }

        [Test]
        public void Sample_TimeSpan_ThrowsException_WhenLowerBoundGreaterThanUpperBound()
        {
            TimeSpan lowerbound = TimeSpan.FromMinutes(60);
            TimeSpan upperbound = TimeSpan.FromMinutes(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => UniformHelper.Sample(_random, lowerbound, upperbound));
        }

        [Test]
        public void Sample_Generic_ReturnsElementFromCandidates()
        {
            var candidates = new List<int> { 1, 2, 3, 4, 5 };

            int result = UniformHelper.Sample(_random, candidates);

            Assert.That(candidates.Contains(result), Is.True);
        }

        [Test]
        public void Sample_Generic_ReturnsDefault_WhenCandidatesEmpty()
        {
            var candidates = new List<int>();

            int result = UniformHelper.Sample(_random, candidates);

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void Sample_Generic_ReturnsNull_WhenCandidatesEmptyAndTypeIsReference()
        {
            var candidates = new List<string>();

            string result = UniformHelper.Sample(_random, candidates);

            Assert.That(result, Is.Null);
        }
    }
}