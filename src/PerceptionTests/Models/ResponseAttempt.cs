using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class ResponseAttempt
    {
        [JsonProperty("attemptNumber", Order = 1)]
        public int AttemptNumber { get; set; }

        [JsonProperty("rawResponseTimeMilliseconds", Order = 2)]
        public double RawResponseTimeMilliseconds { get; set; }

        [JsonProperty("mappedToneDurationMilliseconds", Order = 3)]
        public double? MappedToneDurationMilliseconds { get; set; }

        [JsonProperty("responseCapturedWithinStimulus", Order = 4)]
        public bool ResponseCapturedWithinStimulus
        {
            get { return MappedToneDurationMilliseconds.HasValue; }
        }
    }
}
