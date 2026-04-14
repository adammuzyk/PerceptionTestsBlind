using System;
using System.Configuration;
using System.IO;

namespace PerceptionTests.Services
{
    public class RuntimeSettings
    {
        public string ResultPath { get; private set; }

        public string WaveFilePath { get; private set; }

        public string ExperimentConfigurationPath { get; private set; }

        public string QuestionnaireConfigurationPath { get; private set; }

        public static RuntimeSettings Load()
        {
            return Create(
                ConfigurationManager.AppSettings["ResultPath"],
                ConfigurationManager.AppSettings["WaveFilePath"],
                ConfigurationManager.AppSettings["ExperimentConfigurationPath"],
                ConfigurationManager.AppSettings["QuestionnaireConfigurationPath"],
                AppContext.BaseDirectory);
        }

        internal static RuntimeSettings Create(
            string resultPath,
            string waveFilePath,
            string experimentConfigurationPath,
            string questionnaireConfigurationPath,
            string baseDirectory)
        {
            return new RuntimeSettings
            {
                ResultPath = ResolveRequiredDirectorySetting("ResultPath", resultPath, baseDirectory),
                WaveFilePath = ResolveRequiredDirectorySetting("WaveFilePath", waveFilePath, baseDirectory),
                ExperimentConfigurationPath = ResolveRequiredDirectorySetting("ExperimentConfigurationPath", experimentConfigurationPath, baseDirectory),
                QuestionnaireConfigurationPath = ResolveRequiredDirectorySetting("QuestionnaireConfigurationPath", questionnaireConfigurationPath, baseDirectory)
            };
        }

        internal static string ResolveRequiredDirectorySetting(string key, string configuredValue, string baseDirectory)
        {
            if (string.IsNullOrWhiteSpace(configuredValue))
            {
                throw new InvalidOperationException("Missing required appSetting: " + key);
            }

            if (!Path.IsPathRooted(configuredValue) && string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new InvalidOperationException("Missing base directory for relative appSetting: " + key);
            }

            var absolutePath = Path.IsPathRooted(configuredValue)
                ? configuredValue
                : Path.GetFullPath(Path.Combine(baseDirectory, configuredValue));

            return absolutePath;
        }
    }
}
