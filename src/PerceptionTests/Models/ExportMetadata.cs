using System;
using Newtonsoft.Json;
using PerceptionTests.Music;
using PerceptionTests.Services;

namespace PerceptionTests.Models
{
    public class ExportMetadata
    {
        [JsonProperty("applicationVersion", Order = 1)]
        public string ApplicationVersion { get; set; }

        [JsonProperty("exportedAtUtc", Order = 2)]
        public DateTime? ExportedAtUtc { get; set; }

        [JsonProperty("workstationName", Order = 3)]
        public string WorkstationName { get; set; }

        [JsonProperty("sessionId", Order = 4)]
        public string SessionId { get; set; }

        [JsonProperty("questionnaireVersion", Order = 5)]
        public string QuestionnaireVersion { get; set; }

        [JsonProperty("experimentConfiguration", Order = 6)]
        public ExperimentConfiguration ExperimentConfiguration { get; set; }

        public static ExportMetadata CreateForRun(string sessionId, DateTime exportedAtUtc, string questionnaireVersion)
        {
            return new ExportMetadata
            {
                ApplicationVersion = ApplicationInfo.Version,
                ExportedAtUtc = exportedAtUtc,
                WorkstationName = Environment.MachineName,
                SessionId = sessionId,
                QuestionnaireVersion = questionnaireVersion,
                ExperimentConfiguration = ExperimentCatalog.CreateExperimentConfiguration()
            };
        }
    }
}
