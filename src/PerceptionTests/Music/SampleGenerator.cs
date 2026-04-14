using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionTests.Models;

namespace PerceptionTests.Music
{
    public class SampleGenerator
    {
        private readonly ushort _baseVolume = (ushort.MaxValue - 1) / 2;

        public Sound[] CreateSample(Session session)
        {
            var configuration = ExperimentCatalog.GetSessionConfiguration(session);
            var sample = CreateSample(
                configuration.StartToneDurationMilliseconds,
                configuration.EndToneDurationMilliseconds,
                configuration.NominalSampleDurationMilliseconds,
                _baseVolume,
                configuration.FrequenciesHz.ToArray());

            if (configuration.LowFrequencyGainBelowHz.HasValue && configuration.LowFrequencyGainMultiplier.HasValue)
            {
                for (int i = 0; i < sample.Length; i++)
                {
                    if (sample[i].Frequency < configuration.LowFrequencyGainBelowHz.Value)
                    {
                        sample[i].Volume = (ushort)(sample[i].Volume * configuration.LowFrequencyGainMultiplier.Value);
                    }
                }
            }

            return sample;
        }

        public Sound[] CreateSample(int msStartSoundDuration, int msEndSoundDuration, int msSampleDuration, ushort volume = 16383, params double[] frequencies)
        {
            double actualTime = 0;
            var notes = new List<Sound>();
            while (actualTime < msSampleDuration)
            {
                var actualDuration = GetDuration(actualTime, msSampleDuration, msStartSoundDuration, msEndSoundDuration);
                actualDuration = Math.Round(actualDuration, 2);
                foreach (var frequency in frequencies)
                {
                    notes.Add(new Sound(frequency, actualDuration, volume));
                    actualTime += actualDuration;
                }
            }

            return notes.ToArray();
        }

        private enum AccelerationType
        {
            Linear,
            Logarithmic,
            Hyperbolic,
            Sqrt,
            Root3,
            ArcTan
        }

        private double GetDuration(double actualTime, double msSampleDuration, double msStartSoundDuration,
            double msEndSoundDuration, AccelerationType type = AccelerationType.Hyperbolic)
        {
            double from0To1 = actualTime / msSampleDuration;
            double converted;
            switch (type)
            {
                case AccelerationType.Linear:
                    converted = from0To1;
                    break;
                case AccelerationType.Logarithmic:
                    // Compress the early part of the run so tone shortening happens later.
                    converted = (Math.Log((3.0 * from0To1) + 0.5) - Math.Log(0.5)) / (Math.Log(3.5) - Math.Log(0.5));
                    break;
                case AccelerationType.Hyperbolic:
                    // This is the historical mapping used by the experiment and the external JSON config.
                    // It shortens tones quickly near the beginning and then tapers toward the end.
                    converted = ((-1.0 / ((3.0 * from0To1) + 1.0)) + 1.0) * (4.0 / 3.0);
                    break;
                case AccelerationType.Sqrt:
                    converted = Math.Sqrt(from0To1);
                    break;
                case AccelerationType.Root3:
                    converted = Math.Pow(from0To1, 1.0 / 3.0);
                    break;
                case AccelerationType.ArcTan:
                    converted = Math.Atan(3 * from0To1) / Math.Atan(3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return GetLinearValue(converted, 1, msStartSoundDuration, msEndSoundDuration);
        }

        private double GetLinearValue(double actualParam, double maxParam, double startValue, double endValue)
        {
            return startValue + ((endValue - startValue) * actualParam / maxParam);
        }
    }
}
