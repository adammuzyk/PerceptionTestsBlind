using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class TestModel
    {
        [JsonProperty("metadata", Order = 1)]
        public ExportMetadata Metadata { get; set; }

        [JsonProperty("questionnaire", Order = 2)]
        public QuestionnaireData Questionnaire { get; set; }

        [JsonProperty("Experiment1", Order = 11)]
        public SessionResult Experiment1 { get; set; }

        [JsonProperty("Experiment2", Order = 12)]
        public SessionResult Experiment2 { get; set; }

        [JsonProperty("Experiment3", Order = 13)]
        public SessionResult Experiment3 { get; set; }
    }
}
