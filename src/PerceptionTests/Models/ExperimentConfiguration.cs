using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class ExperimentConfiguration
    {
        [JsonProperty("experiment1", Order = 1)]
        public IReadOnlyList<SessionConfiguration> Experiment1 { get; set; }

        [JsonProperty("experiment2", Order = 2)]
        public IReadOnlyList<SessionConfiguration> Experiment2 { get; set; }

        [JsonProperty("experiment3", Order = 3)]
        public IReadOnlyList<SessionConfiguration> Experiment3 { get; set; }
    }

    public class SessionConfiguration
    {
        [JsonProperty("sessionId", Order = 1)]
        public string SessionId { get; set; }

        [JsonProperty("startToneDurationMilliseconds", Order = 2)]
        public int StartToneDurationMilliseconds { get; set; }

        [JsonProperty("endToneDurationMilliseconds", Order = 3)]
        public int EndToneDurationMilliseconds { get; set; }

        [JsonProperty("nominalSampleDurationMilliseconds", Order = 4)]
        public int NominalSampleDurationMilliseconds { get; set; }

        [JsonProperty("durationMapping", Order = 5)]
        public string DurationMapping { get; set; }

        [JsonProperty("frequenciesHz", Order = 6)]
        public IReadOnlyList<double> FrequenciesHz { get; set; }

        [JsonProperty("lowFrequencyGainBelowHz", Order = 7)]
        public double? LowFrequencyGainBelowHz { get; set; }

        [JsonProperty("lowFrequencyGainMultiplier", Order = 8)]
        public double? LowFrequencyGainMultiplier { get; set; }

        [JsonProperty("attackReleaseMilliseconds", Order = 9)]
        public int AttackReleaseMilliseconds { get; set; }

        [JsonProperty("requiredValidResponses", Order = 10)]
        public int RequiredValidResponses { get; set; }
    }
}
