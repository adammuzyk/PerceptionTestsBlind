using System;
using System.Linq;
using PerceptionTests.Domain;
using PerceptionTests.Music;
using PerceptionTests.Services;
using PerceptionTests.ViewModels;
using Xunit;

namespace PerceptionTests.Tests
{
    public class RepositoryConfigurationSmokeTests
    {
        [Fact]
        [Trait("Suite", "Configuration")]
        public void ReleaseExperimentConfiguration_LoadsAndValidates()
        {
            var configurationPath = TestPathHelper.GetReleaseExperimentConfigurationPath();

            var configuration = ExperimentCatalog.LoadConfigurationFromFile(configurationPath);
            var lookup = ExperimentCatalog.BuildSessionLookup(configuration);
            ExperimentCatalog.ValidateConfiguration(configuration, lookup);

            Assert.Equal(3, configuration.Experiment1.Count);
            Assert.Equal(3, configuration.Experiment2.Count);
            Assert.Equal(3, configuration.Experiment3.Count);

            var configuredSessions = lookup.Keys.OrderBy(session => session).ToArray();
            var expectedSessions = Enum.GetValues(typeof(Session)).Cast<Session>().OrderBy(session => session).ToArray();
            Assert.Equal(expectedSessions, configuredSessions);
        }

        [Fact]
        [Trait("Suite", "Configuration")]
        public void ReleaseQuestionnaireConfiguration_LoadsAndValidates()
        {
            var configurationPath = TestPathHelper.GetReleaseQuestionnaireConfigurationPath();

            var configuration = QuestionnaireCatalog.LoadConfigurationFromFile(configurationPath);
            QuestionnaireCatalog.ValidateConfiguration(configuration);

            Assert.False(string.IsNullOrWhiteSpace(configuration.Musician.Version));
            Assert.False(string.IsNullOrWhiteSpace(configuration.NonMusician.Version));
            Assert.Equal(QuestionnaireFieldIds.MusicianOrderedFields, configuration.Musician.Fields.Select(field => field.FieldId));
            Assert.Equal(QuestionnaireFieldIds.NonMusicianOrderedFields, configuration.NonMusician.Fields.Select(field => field.FieldId));
        }

        [Fact]
        [Trait("Suite", "Configuration")]
        public void ReleaseQuestionnaireConfiguration_CanBeRenderedIntoFormViewModel()
        {
            var configurationPath = TestPathHelper.GetReleaseQuestionnaireConfigurationPath();
            var configuration = QuestionnaireCatalog.LoadConfigurationFromFile(configurationPath);
            QuestionnaireCatalog.ValidateConfiguration(configuration);

            var musicianForm = QuestionnaireFormViewModelFactory.Create(configuration.Musician);
            var nonMusicianForm = QuestionnaireFormViewModelFactory.Create(configuration.NonMusician);

            Assert.Equal(configuration.Musician.Fields.Count, musicianForm.Fields.Count);
            Assert.Equal(configuration.NonMusician.Fields.Count, nonMusicianForm.Fields.Count);
            Assert.All(musicianForm.Fields, field => Assert.NotNull(field));
            Assert.All(nonMusicianForm.Fields, field => Assert.NotNull(field));
        }
    }
}
