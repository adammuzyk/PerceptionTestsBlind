using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionTests.Models;
using PerceptionTests.Music;
using Xunit;

namespace PerceptionTests.Tests
{
    public class ExperimentCatalogTests
    {
        [Fact]
        public void CreateExperimentConfiguration_DefinesAllExpectedSessions()
        {
            ExperimentCatalogTestHelper.EnsureInitialized();
            var configuration = ExperimentCatalog.CreateExperimentConfiguration();

            var configuredSessions = configuration.Experiment1
                .Concat(configuration.Experiment2)
                .Concat(configuration.Experiment3)
                .Select(session => session.SessionId)
                .OrderBy(session => session)
                .ToList();
            var expectedSessions = Enum.GetValues(typeof(Session))
                .Cast<Session>()
                .Select(session => session.ToString())
                .OrderBy(session => session)
                .ToList();

            Assert.Equal(expectedSessions, configuredSessions);
        }

        [Fact]
        public void ValidateConfiguration_ThrowsWhenExpectedSessionIsMissing()
        {
            var configuration = new ExperimentConfiguration
            {
                Experiment1 = Array.Empty<SessionConfiguration>(),
                Experiment2 = Array.Empty<SessionConfiguration>(),
                Experiment3 = Array.Empty<SessionConfiguration>()
            };
            var lookup = new Dictionary<Session, SessionConfiguration>();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ExperimentCatalog.ValidateConfiguration(configuration, lookup));

            Assert.Contains("does not define the expected set of sessions", exception.Message);
        }

        [Fact]
        public void ValidateSessionConfiguration_ThrowsWhenTimingRangeIsInvalid()
        {
            var invalidConfiguration = new SessionConfiguration
            {
                SessionId = Session.Test_1_1.ToString(),
                StartToneDurationMilliseconds = 40,
                EndToneDurationMilliseconds = 80,
                NominalSampleDurationMilliseconds = 40000,
                FrequenciesHz = new[] { 440.0 }
            };

            var exception = Assert.Throws<InvalidOperationException>(
                () => ExperimentCatalog.ValidateSessionConfiguration(Session.Test_1_1, invalidConfiguration));

            Assert.Contains("start tone duration must be greater than or equal to end tone duration", exception.Message);
        }

        [Fact]
        public void ValidateSessionConfiguration_ThrowsWhenFrequenciesAreMissing()
        {
            var invalidConfiguration = new SessionConfiguration
            {
                SessionId = Session.Test_1_2.ToString(),
                StartToneDurationMilliseconds = 400,
                EndToneDurationMilliseconds = 40,
                NominalSampleDurationMilliseconds = 40000,
                FrequenciesHz = Array.Empty<double>()
            };

            var exception = Assert.Throws<InvalidOperationException>(
                () => ExperimentCatalog.ValidateSessionConfiguration(Session.Test_1_2, invalidConfiguration));

            Assert.Contains("must define at least one frequency", exception.Message);
        }

        [Theory]
        [InlineData("linear")]
        [InlineData("logarithmic")]
        [InlineData("hyperbolic")]
        [InlineData("sqrt")]
        [InlineData("root3")]
        [InlineData("arctan")]
        public void ValidateSessionConfiguration_AcceptsSupportedDurationMappings(string durationMapping)
        {
            var configuration = CreateValidSessionConfiguration(durationMapping);

            ExperimentCatalog.ValidateSessionConfiguration(Session.Test_1_1, configuration);
        }

        [Fact]
        public void ValidateSessionConfiguration_ThrowsWhenDurationMappingIsUnsupported()
        {
            var configuration = CreateValidSessionConfiguration("unsupported");

            var exception = Assert.Throws<InvalidOperationException>(
                () => ExperimentCatalog.ValidateSessionConfiguration(Session.Test_1_1, configuration));

            Assert.Contains("Unsupported durationMapping", exception.Message);
            Assert.Contains("linear", exception.Message);
            Assert.Contains("hyperbolic", exception.Message);
        }

        [Fact]
        public void LoadConfigurationFromFile_LoadsResearcherEditableJson()
        {
            var configurationPath = ExperimentCatalogTestHelper.EnsureInitialized();

            var configuration = ExperimentCatalog.LoadConfigurationFromFile(configurationPath);

            Assert.NotNull(configuration);
            Assert.Equal("Test_1_1", configuration.Experiment1[0].SessionId);
            Assert.Equal(3, configuration.Experiment3[0].RequiredValidResponses);
        }

        private static SessionConfiguration CreateValidSessionConfiguration(string durationMapping)
        {
            return new SessionConfiguration
            {
                SessionId = Session.Test_1_1.ToString(),
                StartToneDurationMilliseconds = 400,
                EndToneDurationMilliseconds = 40,
                NominalSampleDurationMilliseconds = 40000,
                DurationMapping = durationMapping,
                FrequenciesHz = new[] { 440.0 },
                AttackReleaseMilliseconds = 10,
                RequiredValidResponses = 3
            };
        }
    }
}
