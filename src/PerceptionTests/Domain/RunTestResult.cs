using System.Collections.Generic;
using System.Linq;

namespace PerceptionTests.Domain
{
    public class RunTestResult
    {
        private readonly List<RunResponseAttempt> _attempts = new List<RunResponseAttempt>();

        public IReadOnlyList<RunResponseAttempt> Attempts
        {
            get { return _attempts; }
        }

        public int ValidResponseCount
        {
            get { return _attempts.Count(a => a.MappedToneDurationMilliseconds.HasValue); }
        }

        public int NonResponseCount
        {
            get { return _attempts.Count(a => !a.MappedToneDurationMilliseconds.HasValue); }
        }

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

        public void AddAttempt(RunResponseAttempt attempt)
        {
            attempt.AttemptNumber = _attempts.Count + 1;
            _attempts.Add(attempt);
        }
    }
}
