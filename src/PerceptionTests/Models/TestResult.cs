using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class TestResult
    {
        private readonly List<ResponseAttempt> _attempts = new List<ResponseAttempt>();

        [JsonProperty("attempts", Order = 1)]
        public List<ResponseAttempt> Attempts
        {
            get { return _attempts; }
        }

        [JsonProperty("validResponseCount", Order = 2)]
        public int ValidResponseCount
        {
            get { return _attempts.Count(a => a.MappedToneDurationMilliseconds.HasValue); }
        }

        [JsonProperty("nonResponseCount", Order = 3)]
        public int NonResponseCount
        {
            get { return _attempts.Count(a => !a.MappedToneDurationMilliseconds.HasValue); }
        }

        [JsonProperty("averageMappedToneDurationMilliseconds", Order = 4)]
        public double? AverageMappedToneDurationMilliseconds
        {
            get
            {
                var mappedValues = _attempts
                    .Where(a => a.MappedToneDurationMilliseconds.HasValue)
                    .Select(a => a.MappedToneDurationMilliseconds.Value)
                    .ToList();

                if (mappedValues.Count == 0)
                {
                    return null;
                }

                return mappedValues.Average();
            }
        }

        public void AddAttempt(ResponseAttempt attempt)
        {
            attempt.AttemptNumber = _attempts.Count + 1;
            _attempts.Add(attempt);
        }
    }
}
