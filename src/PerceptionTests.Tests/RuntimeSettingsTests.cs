using System;
using System.IO;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class RuntimeSettingsTests
    {
        [Fact]
        public void Create_ResolvesRelativePathsAgainstBaseDirectory()
        {
            var baseDirectory = Path.Combine("C:\\", "PerceptionTestsBase");

            var settings = RuntimeSettings.Create(
                ".\\results",
                ".\\wave",
                ".\\experiment-config.json",
                ".\\questionnaire-config.json",
                baseDirectory);

            Assert.Equal(Path.GetFullPath(Path.Combine(baseDirectory, ".\\results")), settings.ResultPath);
            Assert.Equal(Path.GetFullPath(Path.Combine(baseDirectory, ".\\wave")), settings.WaveFilePath);
            Assert.Equal(Path.GetFullPath(Path.Combine(baseDirectory, ".\\experiment-config.json")), settings.ExperimentConfigurationPath);
            Assert.Equal(Path.GetFullPath(Path.Combine(baseDirectory, ".\\questionnaire-config.json")), settings.QuestionnaireConfigurationPath);
        }

        [Fact]
        public void Create_PreservesAbsolutePaths()
        {
            var resultPath = Path.Combine(Path.GetTempPath(), "PerceptionTests", "results");
            var wavePath = Path.Combine(Path.GetTempPath(), "PerceptionTests", "wave");
            var configurationPath = Path.Combine(Path.GetTempPath(), "PerceptionTests", "experiment-config.json");
            var questionnaireConfigurationPath = Path.Combine(Path.GetTempPath(), "PerceptionTests", "questionnaire-config.json");
            var settings = RuntimeSettings.Create(
                resultPath,
                wavePath,
                configurationPath,
                questionnaireConfigurationPath,
                "C:\\ignored");

            Assert.Equal(resultPath, settings.ResultPath);
            Assert.Equal(wavePath, settings.WaveFilePath);
            Assert.Equal(configurationPath, settings.ExperimentConfigurationPath);
            Assert.Equal(questionnaireConfigurationPath, settings.QuestionnaireConfigurationPath);
        }

        [Fact]
        public void ResolveRequiredDirectorySetting_ThrowsForMissingValue()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => RuntimeSettings.ResolveRequiredDirectorySetting("ResultPath", null, "C:\\base"));

            Assert.Contains("Missing required appSetting: ResultPath", exception.Message);
        }

        [Fact]
        public void ResolveRequiredDirectorySetting_ThrowsForRelativeValueWithoutBaseDirectory()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => RuntimeSettings.ResolveRequiredDirectorySetting("WaveFilePath", ".\\wave", null));

            Assert.Contains("Missing base directory for relative appSetting: WaveFilePath", exception.Message);
        }
    }
}
