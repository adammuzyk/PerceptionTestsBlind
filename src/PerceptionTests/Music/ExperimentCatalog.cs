using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PerceptionTests.Models;

namespace PerceptionTests.Music
{
    public static class ExperimentCatalog
    {
        private const string DefaultConfigurationFileName = "experiment-config.json";

        private static ExperimentConfiguration _configuration;
        private static IReadOnlyDictionary<Session, SessionConfiguration> _sessionLookup;

        static ExperimentCatalog()
        {
            var defaultConfigurationPath = Path.Combine(AppContext.BaseDirectory, DefaultConfigurationFileName);
            if (File.Exists(defaultConfigurationPath))
            {
                Initialize(defaultConfigurationPath);
            }
        }

        public static void Initialize(string configurationPath)
        {
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                throw new InvalidOperationException("Experiment configuration path is missing.");
            }

            var configuration = LoadConfigurationFromFile(configurationPath);
            var sessionLookup = BuildSessionLookup(configuration);
            ValidateConfiguration(configuration, sessionLookup);

            _configuration = configuration;
            _sessionLookup = sessionLookup;
        }

        public static SessionConfiguration GetSessionConfiguration(Session session)
        {
            EnsureInitialized();

            if (!_sessionLookup.TryGetValue(session, out var configuration))
            {
                throw new InvalidOperationException("Missing experiment configuration for session " + session + ".");
            }

            return configuration;
        }

        public static ExperimentConfiguration CreateExperimentConfiguration()
        {
            EnsureInitialized();
            return _configuration;
        }

        internal static ExperimentConfiguration LoadConfigurationFromFile(string configurationPath)
        {
            if (!File.Exists(configurationPath))
            {
                throw new InvalidOperationException("Experiment configuration file was not found: " + configurationPath);
            }

            var json = File.ReadAllText(configurationPath);
            var configuration = JsonConvert.DeserializeObject<ExperimentConfiguration>(json);
            if (configuration == null)
            {
                throw new InvalidOperationException("Experiment configuration file is empty or invalid JSON: " + configurationPath);
            }

            return configuration;
        }

        internal static IReadOnlyDictionary<Session, SessionConfiguration> BuildSessionLookup(ExperimentConfiguration configuration)
        {
            return EnumerateAllSessions(configuration)
                .ToDictionary(configurationEntry => ParseSession(configurationEntry.SessionId));
        }

        internal static void ValidateConfiguration(
            ExperimentConfiguration configuration,
            IReadOnlyDictionary<Session, SessionConfiguration> sessionLookup)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Experiment configuration is not initialized.");
            }

            var configuredSessions = sessionLookup.Keys.OrderBy(session => session).ToList();
            var expectedSessions = Enum.GetValues(typeof(Session)).Cast<Session>().OrderBy(session => session).ToList();
            if (!configuredSessions.SequenceEqual(expectedSessions))
            {
                throw new InvalidOperationException("Experiment configuration does not define the expected set of sessions.");
            }

            foreach (var entry in sessionLookup)
            {
                ValidateSessionConfiguration(entry.Key, entry.Value);
            }
        }

        internal static void ValidateSessionConfiguration(Session session, SessionConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Session configuration is null for " + session + ".");
            }

            if (configuration.FrequenciesHz == null || configuration.FrequenciesHz.Count == 0)
            {
                throw new InvalidOperationException("Session " + session + " must define at least one frequency.");
            }

            if (configuration.StartToneDurationMilliseconds <= 0 ||
                configuration.EndToneDurationMilliseconds <= 0 ||
                configuration.NominalSampleDurationMilliseconds <= 0)
            {
                throw new InvalidOperationException("Session " + session + " contains non-positive timing values.");
            }

            if (configuration.StartToneDurationMilliseconds < configuration.EndToneDurationMilliseconds)
            {
                throw new InvalidOperationException("Session " + session + " start tone duration must be greater than or equal to end tone duration.");
            }

            if (configuration.RequiredValidResponses <= 0)
            {
                throw new InvalidOperationException("Session " + session + " must require at least one valid response.");
            }

            if (configuration.AttackReleaseMilliseconds < 0)
            {
                throw new InvalidOperationException("Session " + session + " attack/release duration cannot be negative.");
            }

            if (!string.Equals(configuration.DurationMapping, "hyperbolic", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Session " + session + " uses unsupported durationMapping '" + configuration.DurationMapping + "'.");
            }
        }

        private static IEnumerable<SessionConfiguration> EnumerateAllSessions(ExperimentConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Experiment configuration is not initialized.");
            }

            return EnumerateExperiment("experiment1", configuration.Experiment1)
                .Concat(EnumerateExperiment("experiment2", configuration.Experiment2))
                .Concat(EnumerateExperiment("experiment3", configuration.Experiment3));
        }

        private static IEnumerable<SessionConfiguration> EnumerateExperiment(string experimentName, IReadOnlyList<SessionConfiguration> sessions)
        {
            if (sessions == null)
            {
                throw new InvalidOperationException("Experiment configuration section '" + experimentName + "' is missing.");
            }

            return sessions;
        }

        private static Session ParseSession(string sessionId)
        {
            if (!Enum.TryParse(sessionId, out Session parsedSession))
            {
                throw new InvalidOperationException("Unknown sessionId in experiment configuration: " + sessionId);
            }

            return parsedSession;
        }

        private static void EnsureInitialized()
        {
            if (_configuration == null || _sessionLookup == null)
            {
                throw new InvalidOperationException("Experiment catalog has not been initialized.");
            }
        }
    }
}
