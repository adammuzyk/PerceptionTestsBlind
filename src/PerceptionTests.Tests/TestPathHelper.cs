using System;
using System.IO;

namespace PerceptionTests.Tests
{
    internal static class TestPathHelper
    {
        public static string FindRepositoryRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                if (Directory.Exists(Path.Combine(current.FullName, "src", "PerceptionTests")) &&
                    File.Exists(Path.Combine(current.FullName, "README.md")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Repository root could not be located from the current test base directory.");
        }

        public static string GetReleaseExperimentConfigurationPath()
        {
            return Path.Combine(FindRepositoryRoot(), "src", "PerceptionTests", "experiment-config.json");
        }

        public static string GetReleaseQuestionnaireConfigurationPath()
        {
            return Path.Combine(FindRepositoryRoot(), "src", "PerceptionTests", "questionnaire-config.json");
        }
    }
}
