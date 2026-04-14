using System.Collections.Generic;
using PerceptionTests.Domain;
using PerceptionTests.Models;

namespace PerceptionTests.Music
{
    public static class ResponseMapper
    {
        public static RunResponseAttempt CreateAttempt(IReadOnlyList<Sound> sample, double elapsedMilliseconds)
        {
            return new RunResponseAttempt
            {
                RawResponseTimeMilliseconds = elapsedMilliseconds,
                MappedToneDurationMilliseconds = MapElapsedMillisecondsToToneDuration(sample, elapsedMilliseconds)
            };
        }

        public static double? MapElapsedMillisecondsToToneDuration(IReadOnlyList<Sound> sample, double elapsedMilliseconds)
        {
            var remainingMilliseconds = elapsedMilliseconds;
            for (int i = 0; i < sample.Count; i++)
            {
                if (remainingMilliseconds < sample[i].Duration)
                {
                    return sample[i].Duration;
                }

                remainingMilliseconds -= sample[i].Duration;
            }

            return null;
        }
    }
}
