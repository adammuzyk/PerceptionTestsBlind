using System;
using System.IO;
using Newtonsoft.Json.Linq;
using PerceptionTests.Domain;
using PerceptionTests.Music;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class ResultPersistenceServiceTests
    {
        [Fact]
        [Trait("Suite", "Persistence")]
        public void SaveCheckpoint_WritesCheckpointJsonUsingExportSchema()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var configurationPath = ExperimentCatalogTestHelper.EnsureInitialized();
                var questionnaireConfigurationPath = ExperimentCatalogTestHelper.EnsureQuestionnaireInitialized();
                var service = new ResultPersistenceService(
                    RuntimeSettings.Create(
                        tempDirectory,
                        Path.Combine(tempDirectory, "wave"),
                        configurationPath,
                        questionnaireConfigurationPath,
                        tempDirectory));
                var state = CreateStateWithResult();

                var result = service.SaveCheckpoint(state);

                Assert.True(result.Success);
                Assert.Equal(
                    Path.Combine(tempDirectory, "checkpoints", "checkpoint_" + state.SessionId + ".json"),
                    result.FilePath);
                Assert.True(File.Exists(result.FilePath));

                var json = JObject.Parse(File.ReadAllText(result.FilePath));
                var expectedQuestionnaireVersion = QuestionnaireCatalog.GetFormVersion(true);
                Assert.Equal(state.SessionId, (string)json["metadata"]["sessionId"]);
                Assert.NotNull(json["metadata"]["exportedAtUtc"]);
                Assert.Equal(expectedQuestionnaireVersion, (string)json["metadata"]["questionnaireVersion"]);
                Assert.Equal("female", (string)json["questionnaire"]["gender"]);
                Assert.Equal(120.5, (double)json["Experiment1"]["Session1"]["attempts"][0]["rawResponseTimeMilliseconds"]);
            }
            finally
            {
                DeleteDirectoryIfExists(tempDirectory);
            }
        }

        [Fact]
        [Trait("Suite", "Persistence")]
        public void SaveFinal_GeneratesNextSequentialFileAndDeletesCheckpoint()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var configurationPath = ExperimentCatalogTestHelper.EnsureInitialized();
                var questionnaireConfigurationPath = ExperimentCatalogTestHelper.EnsureQuestionnaireInitialized();
                File.WriteAllText(Path.Combine(tempDirectory, "testResult_0001.json"), "{}");
                var service = new ResultPersistenceService(
                    RuntimeSettings.Create(
                        tempDirectory,
                        Path.Combine(tempDirectory, "wave"),
                        configurationPath,
                        questionnaireConfigurationPath,
                        tempDirectory));
                var state = CreateStateWithResult();

                var checkpointResult = service.SaveCheckpoint(state);
                Assert.True(checkpointResult.Success);
                Assert.True(File.Exists(checkpointResult.FilePath));

                var finalResult = service.SaveFinal(state);

                Assert.True(finalResult.Success);
                Assert.Equal(Path.Combine(tempDirectory, "testResult_0002.json"), finalResult.FilePath);
                Assert.True(File.Exists(finalResult.FilePath));
                Assert.False(File.Exists(checkpointResult.FilePath));

                var json = JObject.Parse(File.ReadAllText(finalResult.FilePath));
                var expectedQuestionnaireVersion = QuestionnaireCatalog.GetFormVersion(true);
                Assert.Equal(state.SessionId, (string)json["metadata"]["sessionId"]);
                Assert.Equal(expectedQuestionnaireVersion, (string)json["metadata"]["questionnaireVersion"]);
                Assert.NotNull(json["metadata"]["experimentConfiguration"]);
            }
            finally
            {
                DeleteDirectoryIfExists(tempDirectory);
            }
        }

        [Fact]
        [Trait("Suite", "Persistence")]
        public void SaveFinal_WithReleaseConfiguration_WritesValidStructuredJson()
        {
            var tempDirectory = CreateTempDirectory();

            try
            {
                var configurationPath = TestPathHelper.GetReleaseExperimentConfigurationPath();
                var questionnaireConfigurationPath = TestPathHelper.GetReleaseQuestionnaireConfigurationPath();
                ExperimentCatalog.Initialize(configurationPath);
                QuestionnaireCatalog.Initialize(questionnaireConfigurationPath);

                var service = new ResultPersistenceService(
                    RuntimeSettings.Create(
                        tempDirectory,
                        Path.Combine(tempDirectory, "wave"),
                        configurationPath,
                        questionnaireConfigurationPath,
                        tempDirectory));

                var state = CreateStateWithResult();
                var finalResult = service.SaveFinal(state);

                Assert.True(finalResult.Success);
                Assert.True(File.Exists(finalResult.FilePath));

                var json = JObject.Parse(File.ReadAllText(finalResult.FilePath));
                Assert.Equal(state.SessionId, (string)json["metadata"]["sessionId"]);
                Assert.False(string.IsNullOrWhiteSpace((string)json["metadata"]["questionnaireVersion"]));
                Assert.NotNull(json["metadata"]["experimentConfiguration"]["experiment1"]);
                Assert.NotNull(json["Experiment1"]["Session1"]["attempts"]);
            }
            finally
            {
                DeleteDirectoryIfExists(tempDirectory);
            }
        }

        private static ExperimentRunState CreateStateWithResult()
        {
            var state = new ExperimentRunState();
            state.Questionnaire.IsMusician = true;
            state.Questionnaire.Gender = "female";
            state.Questionnaire.Age = 29;
            state.Questionnaire.Handedness = "right";

            var result = new RunTestResult();
            result.AddAttempt(new RunResponseAttempt
            {
                RawResponseTimeMilliseconds = 120.5,
                MappedToneDurationMilliseconds = 80
            });

            state.AddResult(Session.Test_1_1, result);
            return state;
        }

        private static string CreateTempDirectory()
        {
            var path = Path.Combine(Path.GetTempPath(), "PerceptionTests.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
