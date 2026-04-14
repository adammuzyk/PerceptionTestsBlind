using System;
using System.IO;
using Newtonsoft.Json;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class QuestionnaireCatalogTests
    {
        private const string TestVersion = "test-version";

        [Fact]
        public void LoadConfigurationFromFile_LoadsResearcherEditableQuestionnaireJson()
        {
            var configurationPath = CreateQuestionnaireConfigurationFile(CreateValidConfiguration());

            try
            {
                var configuration = QuestionnaireCatalog.LoadConfigurationFromFile(configurationPath);

                Assert.NotNull(configuration);
                Assert.Equal(QuestionnaireFieldIds.Gender, configuration.Musician.Fields[0].FieldId);
                Assert.Equal(QuestionnaireFieldType.IntegerStepper, configuration.Musician.Fields[1].Type);
                Assert.Equal(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, configuration.NonMusician.Fields[3].FieldId);
                Assert.Equal(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, configuration.NonMusician.Fields[4].VisibleWhen.FieldId);
            }
            finally
            {
                DeleteFileIfExists(configurationPath);
            }
        }

        [Fact]
        public void ValidateConfiguration_ThrowsWhenExpectedFieldSetDoesNotMatch()
        {
            var configuration = new QuestionnaireConfiguration
            {
                Musician = new QuestionnaireFormConfiguration
                {
                    Version = TestVersion,
                    Fields = Array.Empty<QuestionnaireFieldConfiguration>()
                },
                NonMusician = CreateValidConfiguration().NonMusician
            };

            var exception = Assert.Throws<InvalidOperationException>(
                () => QuestionnaireCatalog.ValidateConfiguration(configuration));

            Assert.Contains("does not define the expected ordered field set", exception.Message);
        }

        [Fact]
        public void ValidateConfiguration_ThrowsWhenChoiceFieldHasEmptyOptions()
        {
            var configuration = CreateValidConfiguration();
            var musicianFields = new QuestionnaireFieldConfiguration[]
            {
                new QuestionnaireFieldConfiguration
                {
                    FieldId = QuestionnaireFieldIds.Gender,
                    Type = QuestionnaireFieldType.Choice,
                    Label = "Gender",
                    Required = true,
                    Options = Array.Empty<QuestionnaireOptionConfiguration>()
                },
                Stepper(QuestionnaireFieldIds.Age, "Age", 1, 100, 1),
                Choice(QuestionnaireFieldIds.Handedness, "Are you left- or right-handed?", "right", "Right-handed", "left", "Left-handed"),
                Text(QuestionnaireFieldIds.MusicalEducationDescription, "What musical education do you have?"),
                Stepper(QuestionnaireFieldIds.InstrumentLearningStartAge, "At what age did you start learning to play a musical instrument?", 1, 100, 1),
                Stepper(QuestionnaireFieldIds.InstrumentPracticeYears, "For how many years has the instrument been consistently practiced?", 1, 100, 1),
                Choice(QuestionnaireFieldIds.HasAbsolutePitch, "Do you have absolute pitch?", "True", "Yes", "False", "No"),
                Text(QuestionnaireFieldIds.PrimaryPerformanceGenre, "What kind of music do you perform most often?")
            };
            configuration.Musician = new QuestionnaireFormConfiguration { Version = TestVersion, Fields = musicianFields };

            var exception = Assert.Throws<InvalidOperationException>(
                () => QuestionnaireCatalog.ValidateConfiguration(configuration));

            Assert.Contains("defines an empty options list", exception.Message);
        }

        [Fact]
        public void ValidateConfiguration_ThrowsWhenIntegerStepperFieldIsMissingRangeMetadata()
        {
            var configuration = CreateValidConfiguration();
            configuration.Musician.Fields[1].Min = null;

            var exception = Assert.Throws<InvalidOperationException>(
                () => QuestionnaireCatalog.ValidateConfiguration(configuration));

            Assert.Contains("must define min, max, and step", exception.Message);
        }

        [Fact]
        public void ValidateConfiguration_ThrowsWhenVisibleWhenReferencesUnknownField()
        {
            var configuration = CreateValidConfiguration();
            configuration.NonMusician.Fields[4].VisibleWhen = new QuestionnaireVisibilityRuleConfiguration
            {
                FieldId = "unknownField",
                EqualsValue = "True"
            };

            var exception = Assert.Throws<InvalidOperationException>(
                () => QuestionnaireCatalog.ValidateConfiguration(configuration));

            Assert.Contains("references unknown visibleWhen field", exception.Message);
        }

        private static QuestionnaireConfiguration CreateValidConfiguration()
        {
            return new QuestionnaireConfiguration
            {
                Musician = new QuestionnaireFormConfiguration
                {
                    Version = TestVersion,
                    Fields = new QuestionnaireFieldConfiguration[]
                    {
                        Choice(QuestionnaireFieldIds.Gender, "Gender", "male", "Male", "female", "Female"),
                        Stepper(QuestionnaireFieldIds.Age, "Age", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.Handedness, "Are you left- or right-handed?", "right", "Right-handed", "left", "Left-handed"),
                        Text(QuestionnaireFieldIds.MusicalEducationDescription, "What musical education do you have?"),
                        Stepper(QuestionnaireFieldIds.InstrumentLearningStartAge, "At what age did you start learning to play a musical instrument?", 1, 100, 1),
                        Stepper(QuestionnaireFieldIds.InstrumentPracticeYears, "For how many years has the instrument been consistently practiced?", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.HasAbsolutePitch, "Do you have absolute pitch?", "True", "Yes", "False", "No"),
                        Text(QuestionnaireFieldIds.PrimaryPerformanceGenre, "What kind of music do you perform most often?")
                    }
                },
                NonMusician = new QuestionnaireFormConfiguration
                {
                    Version = TestVersion,
                    Fields = new QuestionnaireFieldConfiguration[]
                    {
                        Choice(QuestionnaireFieldIds.Gender, "Gender", "male", "Male", "female", "Female"),
                        Stepper(QuestionnaireFieldIds.Age, "Age", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.Handedness, "Are you left- or right-handed?", "right", "Right-handed", "left", "Left-handed"),
                        Choice(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, "Have you ever taken part in amateur music performance?", "True", "Yes", "False", "No"),
                        Text(
                            QuestionnaireFieldIds.AmateurMusicActivityDetails,
                            "Please describe the musical activity and how long you took part in it.",
                            new QuestionnaireVisibilityRuleConfiguration
                            {
                                FieldId = QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience,
                                EqualsValue = "True"
                            }),
                        Text(QuestionnaireFieldIds.PreferredListeningMusic, "What music do you listen to most often?"),
                        Text(QuestionnaireFieldIds.StudyYearAndSpecialization, "What is your year of study and specialization?")
                    }
                }
            };
        }

        private static QuestionnaireFieldConfiguration Text(
            string fieldId,
            string label,
            QuestionnaireVisibilityRuleConfiguration visibleWhen = null)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.Text,
                Label = label,
                Required = true,
                VisibleWhen = visibleWhen
            };
        }

        private static QuestionnaireFieldConfiguration Choice(
            string fieldId,
            string label,
            string firstValue,
            string firstLabel,
            string secondValue,
            string secondLabel)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.Choice,
                Label = label,
                Required = true,
                Options = new[]
                {
                    new QuestionnaireOptionConfiguration { Value = firstValue, Label = firstLabel },
                    new QuestionnaireOptionConfiguration { Value = secondValue, Label = secondLabel }
                }
            };
        }

        private static QuestionnaireFieldConfiguration Stepper(string fieldId, string label, int min, int max, int step)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.IntegerStepper,
                Label = label,
                Required = true,
                Min = min,
                Max = max,
                Step = step
            };
        }

        private static string CreateQuestionnaireConfigurationFile(QuestionnaireConfiguration configuration)
        {
            var directory = Path.Combine(Path.GetTempPath(), "PerceptionTests.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "questionnaire-config.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(configuration, Formatting.Indented));
            return path;
        }

        private static void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);
                File.Delete(path);
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}
