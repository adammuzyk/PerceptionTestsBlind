#define SAVE_FILE

using System;
using System.IO;
using System.Linq;
using System.Media;
using PerceptionTests.Models;
using PerceptionTests.Services;

namespace PerceptionTests
{
    public class WaveGenerator
    {
        private readonly RuntimeSettings _settings;

        public WaveGenerator(RuntimeSettings settings)
        {
            _settings = settings;
        }

        public void PlayBeep(Sound sound, bool async = true)
        {
            PlayBeep(sound.Frequency, (int)sound.Duration, sound.Volume, async);
        }

        public void PlayBeep(double frequency, int msDuration, ushort volume = 16383, bool async = true)
        {
            var mStrm = new MemoryStream();
            var writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(0x46464952);
            writer.Write(fileSize);
            writer.Write(0x45564157);
            writer.Write(0x20746D66);
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164);
            writer.Write(dataChunkSize);
            {
                double theta = frequency * TAU / samplesPerSecond;
                double amp = volume >> 2;
                for (int step = 0; step < samples; step++)
                {
                    short s = (short)(amp * Math.Sin(theta * step));
                    writer.Write(s);
                }
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            var player = new SoundPlayer(mStrm);
            if (async)
            {
                player.Play();
            }
            else
            {
                player.PlaySync();
            }

            writer.Close();
            mStrm.Close();
        }

        public SoundPlayer CreatePlayer(string filename, params Sound[] sounds)
        {
            var allDuration = sounds.Sum(s => s.Duration);
            var mStrm = new MemoryStream();
            var writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int allSamples = (int)(samplesPerSecond * allDuration / 1000);
            int dataChunkSize = allSamples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(0x46464952);
            writer.Write(fileSize);
            writer.Write(0x45564157);
            writer.Write(0x20746D66);
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164);
            writer.Write(dataChunkSize);

            foreach (var sound in sounds)
            {
                var duration = sound.Duration;
                int samples = (int)(samplesPerSecond * duration / 1000);
                double theta = sound.Frequency * TAU / samplesPerSecond;
                double amp = sound.Volume >> 2;
                for (int step = 0; step < samples; step++)
                {
                    var adjustance = GetAdjustance(step, duration, samplesPerSecond);
                    short s = (short)(amp * adjustance * Math.Sin(theta * step));
                    writer.Write(s);
                }
            }

#if SAVE_FILE
            TryPersistWaveFile(filename, mStrm);
#endif

            mStrm.Seek(0, SeekOrigin.Begin);
            var player = new SoundPlayer(mStrm);
            player.Load();
            writer.Close();
            mStrm.Close();
            return player;
        }

        private void TryPersistWaveFile(string filename, MemoryStream stream)
        {
            try
            {
                Directory.CreateDirectory(_settings.WaveFilePath);
                using (var fileStream = File.Create(Path.Combine(_settings.WaveFilePath, filename + ".wav")))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }
            catch
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        private double GetAdjustance(int step, double duration, int samplesPerSecond)
        {
            double timeInMs = step * 1000.0 / samplesPerSecond;
            if (timeInMs < 10)
            {
                return timeInMs / 10;
            }

            var timeToEnd = duration - timeInMs;
            if (timeToEnd < 10)
            {
                return timeToEnd / 10;
            }

            return 1;
        }
    }
}
