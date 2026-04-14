using PerceptionTests.Models;
using PerceptionTests.Music;
using Xunit;

namespace PerceptionTests.Tests
{
    public class ResponseMapperTests
    {
        [Fact]
        public void MapElapsedMillisecondsToToneDuration_ReturnsCurrentToneDuration()
        {
            var sample = new[]
            {
                new Sound(100, 100),
                new Sound(200, 150),
                new Sound(300, 250)
            };

            var mappedDuration = ResponseMapper.MapElapsedMillisecondsToToneDuration(sample, 120);

            Assert.Equal(150, mappedDuration);
        }

        [Fact]
        public void CreateAttempt_StoresRawAndMappedTiming()
        {
            var sample = new[]
            {
                new Sound(100, 100),
                new Sound(200, 150)
            };

            var attempt = ResponseMapper.CreateAttempt(sample, 75);

            Assert.Equal(75, attempt.RawResponseTimeMilliseconds);
            Assert.Equal(100, attempt.MappedToneDurationMilliseconds);
            Assert.True(attempt.ResponseCapturedWithinStimulus);
        }

        [Fact]
        public void CreateAttempt_ReturnsNonResponseWhenElapsedTimeExceedsStimulus()
        {
            var sample = new[]
            {
                new Sound(100, 100),
                new Sound(200, 150)
            };

            var attempt = ResponseMapper.CreateAttempt(sample, 1000);

            Assert.Equal(1000, attempt.RawResponseTimeMilliseconds);
            Assert.Null(attempt.MappedToneDurationMilliseconds);
            Assert.False(attempt.ResponseCapturedWithinStimulus);
        }
    }
}
