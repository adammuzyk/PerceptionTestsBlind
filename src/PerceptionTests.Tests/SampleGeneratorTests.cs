using System.Linq;
using PerceptionTests;
using PerceptionTests.Music;
using Xunit;

namespace PerceptionTests.Tests
{
    public class SampleGeneratorTests
    {
        [Fact]
        public void CreateSample_UsesDecreasingToneDurationsForConfiguredSession()
        {
            ExperimentCatalogTestHelper.EnsureInitialized();
            var generator = new SampleGenerator();

            var sample = generator.CreateSample(Session.Test_1_1);

            Assert.NotEmpty(sample);
            Assert.True(sample.First().Duration >= 399);
            Assert.True(sample.Last().Duration <= 45);

            for (int i = 1; i < sample.Length; i++)
            {
                Assert.True(sample[i].Duration <= sample[i - 1].Duration || sample[i].Frequency != sample[i - 1].Frequency);
            }
        }

        [Fact]
        public void CreateSample_AppliesLowFrequencyGainInExperiment3()
        {
            ExperimentCatalogTestHelper.EnsureInitialized();
            var generator = new SampleGenerator();

            var sample = generator.CreateSample(Session.Test_3_1);

            var lowFrequencySound = sample.First(s => s.Frequency == 350);
            var highFrequencySound = sample.First(s => s.Frequency == 2500);

            Assert.Equal(highFrequencySound.Volume * 2, lowFrequencySound.Volume);
        }
    }
}
