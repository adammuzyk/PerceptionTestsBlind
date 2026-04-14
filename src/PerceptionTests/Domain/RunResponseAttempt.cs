namespace PerceptionTests.Domain
{
    public class RunResponseAttempt
    {
        public int AttemptNumber { get; set; }

        public double RawResponseTimeMilliseconds { get; set; }

        public double? MappedToneDurationMilliseconds { get; set; }

        public bool ResponseCapturedWithinStimulus
        {
            get { return MappedToneDurationMilliseconds.HasValue; }
        }
    }
}
