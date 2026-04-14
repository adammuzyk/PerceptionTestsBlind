using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class SessionResult
    {
        [JsonProperty("Session1")]
        public TestResult Session1 { get; set; }

        [JsonProperty("Session2")]
        public TestResult Session2 { get; set; }

        [JsonProperty("Session3")]
        public TestResult Session3 { get; set; }
    }
}
