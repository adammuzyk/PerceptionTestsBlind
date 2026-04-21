using System.Linq;
using PerceptionTests;
using PerceptionTests.Music;
using PerceptionTests.Models;
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

        [Theory]
        [InlineData(DurationMappingType.Linear)]
        [InlineData(DurationMappingType.Logarithmic)]
        [InlineData(DurationMappingType.Hyperbolic)]
        [InlineData(DurationMappingType.Sqrt)]
        [InlineData(DurationMappingType.Root3)]
        [InlineData(DurationMappingType.ArcTan)]
        public void CreateSample_SupportsConfiguredDurationMappings(DurationMappingType durationMapping)
        {
            var generator = new SampleGenerator();

            var sample = generator.CreateSample(400, 40, 40000, durationMapping, 16383, 440.0);

            Assert.NotEmpty(sample);
            Assert.True(sample.First().Duration >= 399);
            Assert.True(sample.Last().Duration <= 45);
        }

        [Fact]
        public void CreateSample_ProducesDifferentSchedulesForDifferentDurationMappings()
        {
            var generator = new SampleGenerator();

            var linear = generator.CreateSample(400, 40, 40000, DurationMappingType.Linear, 16383, 440.0);
            var logarithmic = generator.CreateSample(400, 40, 40000, DurationMappingType.Logarithmic, 16383, 440.0);
            var hyperbolic = generator.CreateSample(400, 40, 40000, DurationMappingType.Hyperbolic, 16383, 440.0);

            Assert.NotEqual(linear[10].Duration, logarithmic[10].Duration);
            Assert.NotEqual(linear[10].Duration, hyperbolic[10].Duration);
        }

        [Fact]
        public void CreateSample_UsesDurationMappingFromSessionConfiguration()
        {
            var configuration = ExperimentCatalogTestHelper.CreateExperimentConfigurationWithDurationMappings(
                "linear",
                "logarithmic",
                "hyperbolic");

            try
            {
                ExperimentCatalogTestHelper.InitializeExperimentConfiguration(
                    configuration,
                    "experiment-config-custom-mappings");
                var generator = new SampleGenerator();

                var linearSession = generator.CreateSample(Session.Test_1_1);
                var logarithmicSession = generator.CreateSample(Session.Test_1_2);
                var hyperbolicSession = generator.CreateSample(Session.Test_1_3);

                Assert.NotEqual(linearSession[10].Duration, logarithmicSession[10].Duration);
                Assert.NotEqual(linearSession[10].Duration, hyperbolicSession[10].Duration);
                Assert.Equal(
                    generator.CreateSample(400, 40, 40000, DurationMappingType.Linear, 16383, 440.0)[10].Duration,
                    linearSession[10].Duration);
                Assert.Equal(
                    generator.CreateSample(400, 40, 40000, DurationMappingType.Logarithmic, 16383, 440.0)[10].Duration,
                    logarithmicSession[10].Duration);
                Assert.Equal(
                    generator.CreateSample(400, 40, 40000, DurationMappingType.Hyperbolic, 16383, 440.0)[10].Duration,
                    hyperbolicSession[10].Duration);
            }
            finally
            {
                ExperimentCatalogTestHelper.RestoreDefaultExperimentConfiguration();
            }
        }
    }
}
