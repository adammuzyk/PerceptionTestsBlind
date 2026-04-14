using System;
using System.IO;
using Newtonsoft.Json;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.Music;
using PerceptionTests.Services;

namespace PerceptionTests.Tests
{
    internal static class ExperimentCatalogTestHelper
    {
        internal const string TestQuestionnaireVersion = "test-version";
        private static readonly Lazy<string> ConfigurationPath = new Lazy<string>(CreateAndInitializeConfiguration);
        private static readonly Lazy<string> QuestionnaireConfigurationPath = new Lazy<string>(CreateAndInitializeQuestionnaireConfiguration);

        public static string EnsureInitialized()
        {
            EnsureQuestionnaireInitialized();
            return ConfigurationPath.Value;
        }

        public static string EnsureQuestionnaireInitialized()
        {
            return QuestionnaireConfigurationPath.Value;
        }

        private static string CreateAndInitializeConfiguration()
        {
            var configuration = new ExperimentConfiguration
            {
                Experiment1 = new[]
                {
                    CreateSession(Session.Test_1_1, 400, 40, 40000, new[] { 73.416, 110.0 }, null, null),
                    CreateSession(Session.Test_1_2, 400, 40, 40000, new[] { 293.666, 440.0 }, null, null),
                    CreateSession(Session.Test_1_3, 400, 40, 40000, new[] { 1174.7, 1760.0 }, null, null)
                },
                Experiment2 = new[]
                {
                    CreateSession(Session.Test_2_1, 400, 40, 40000, new[] { 329.63, 349.23 }, null, null),
                    CreateSession(Session.Test_2_2, 400, 40, 40000, new[] { 311.13, 415.3 }, null, null),
                    CreateSession(Session.Test_2_3, 400, 40, 40000, new[] { 261.63, 439.88 }, null, null)
                },
                Experiment3 = new[]
                {
                    CreateSession(Session.Test_3_1, 400, 100, 50000, new[] { 2500.0, 350.0, 2000.0, 430.0, 1600.0, 550.0 }, 600.0, 2.0),
                    CreateSession(Session.Test_3_2, 400, 100, 50000, new[] { 550.0, 1600.0, 430.0, 2000.0, 350.0, 2500.0 }, 600.0, 2.0),
                    CreateSession(Session.Test_3_3, 400, 100, 50000, new[] { 2500.0, 550.0, 2000.0, 430.0, 1600.0, 350.0 }, 600.0, 2.0)
                }
            };

            var directory = Path.Combine(Path.GetTempPath(), "PerceptionTests.Tests", "experiment-config");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "experiment-config.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(configuration, Formatting.Indented));
            ExperimentCatalog.Initialize(path);
            return path;
        }

        private static string CreateAndInitializeQuestionnaireConfiguration()
        {
            var configuration = new QuestionnaireConfiguration
            {
                Musician = new QuestionnaireFormConfiguration
                {
                    Version = TestQuestionnaireVersion,
                    Fields = new QuestionnaireFieldConfiguration[]
                    {
                        Choice(QuestionnaireFieldIds.Gender, "Gender", "male", "Male", "female", "Female"),
                        Stepper(QuestionnaireFieldIds.Age, "Age", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.Handedness, "Are you left- or right-handed?", "left", "Left-handed", "right", "Right-handed"),
                        Text(QuestionnaireFieldIds.MusicalEducationDescription, "What musical education do you have?"),
                        Stepper(QuestionnaireFieldIds.InstrumentLearningStartAge, "At what age did you start learning to play a musical instrument?", 1, 100, 1),
                        Stepper(QuestionnaireFieldIds.InstrumentPracticeYears, "For how many years has the instrument been consistently practiced?", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.HasAbsolutePitch, "Do you have absolute pitch?", "True", "Yes", "False", "No"),
                        Text(QuestionnaireFieldIds.PrimaryPerformanceGenre, "What kind of music do you perform most often?")
                    }
                },
                NonMusician = new QuestionnaireFormConfiguration
                {
                    Version = TestQuestionnaireVersion,
                    Fields = new QuestionnaireFieldConfiguration[]
                    {
                        Choice(QuestionnaireFieldIds.Gender, "Gender", "male", "Male", "female", "Female"),
                        Stepper(QuestionnaireFieldIds.Age, "Age", 1, 100, 1),
                        Choice(QuestionnaireFieldIds.Handedness, "Are you left- or right-handed?", "left", "Left-handed", "right", "Right-handed"),
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

            var directory = Path.Combine(Path.GetTempPath(), "PerceptionTests.Tests", "questionnaire-config");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "questionnaire-config.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(configuration, Formatting.Indented));
            QuestionnaireCatalog.Initialize(path);
            return path;
        }

        private static SessionConfiguration CreateSession(
            Session session,
            int startToneDurationMilliseconds,
            int endToneDurationMilliseconds,
            int nominalSampleDurationMilliseconds,
            double[] frequenciesHz,
            double? lowFrequencyGainBelowHz,
            double? lowFrequencyGainMultiplier)
        {
            return new SessionConfiguration
            {
                SessionId = session.ToString(),
                StartToneDurationMilliseconds = startToneDurationMilliseconds,
                EndToneDurationMilliseconds = endToneDurationMilliseconds,
                NominalSampleDurationMilliseconds = nominalSampleDurationMilliseconds,
                DurationMapping = "hyperbolic",
                FrequenciesHz = frequenciesHz,
                LowFrequencyGainBelowHz = lowFrequencyGainBelowHz,
                LowFrequencyGainMultiplier = lowFrequencyGainMultiplier,
                AttackReleaseMilliseconds = 10,
                RequiredValidResponses = 3
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
    }
}
