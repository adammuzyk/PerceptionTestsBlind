using System;

namespace PerceptionTests.Models
{
    public struct Sound
    {
        public Sound(double frequency, double duration, UInt16 volume = 16383)
        {
            Duration = duration;
            Frequency = frequency;
            Volume = volume;
        }

        /// <summary>
        /// duration in ms
        /// </summary>
        public double Duration;

        /// <summary>
        /// frequency in Hz
        /// </summary>
        public double Frequency;

        /// <summary>
        /// volume
        /// </summary>
        public UInt16 Volume;
    }
}
