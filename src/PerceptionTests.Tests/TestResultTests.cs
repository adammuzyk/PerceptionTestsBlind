using PerceptionTests.Models;
using Xunit;

namespace PerceptionTests.Tests
{
    public class TestResultTests
    {
        [Fact]
        public void AddAttempt_AssignsAttemptNumbersAndComputesStatistics()
        {
            var result = new TestResult();

            result.AddAttempt(new ResponseAttempt { RawResponseTimeMilliseconds = 100, MappedToneDurationMilliseconds = 80 });
            result.AddAttempt(new ResponseAttempt { RawResponseTimeMilliseconds = 200, MappedToneDurationMilliseconds = null });
            result.AddAttempt(new ResponseAttempt { RawResponseTimeMilliseconds = 300, MappedToneDurationMilliseconds = 120 });

            Assert.Equal(1, result.Attempts[0].AttemptNumber);
            Assert.Equal(2, result.Attempts[1].AttemptNumber);
            Assert.Equal(3, result.Attempts[2].AttemptNumber);
            Assert.Equal(2, result.ValidResponseCount);
            Assert.Equal(1, result.NonResponseCount);
            Assert.Equal(100, result.AverageMappedToneDurationMilliseconds);
        }
    }
}
