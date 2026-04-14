using System.Diagnostics;
using PerceptionTests.Domain;
using PerceptionTests.Music;
using PerceptionTests.Models;

namespace PerceptionTests
{
    public class Player
    {
        private readonly SampleGenerator _sampleGenerator;
        private readonly WaveGenerator _waveGenerator;
        private readonly Stopwatch _stopwatch;

        private System.Media.SoundPlayer _player;
        private Sound[] _sample;

        public Player(SampleGenerator sampleGenerator, WaveGenerator waveGenerator)
        {
            _sampleGenerator = sampleGenerator;
            _waveGenerator = waveGenerator;
            _stopwatch = new Stopwatch();
        }

        public void PrepareSession(Session session)
        {
            _sample = _sampleGenerator.CreateSample(session);
            _player = _waveGenerator.CreatePlayer(session.ToString(), _sample);
        }

        public void Play()
        {
            _stopwatch.Restart();
            _player.Play();
        }

        public RunResponseAttempt Stop()
        {
            _stopwatch.Stop();
            _player.Stop();
            return ResponseMapper.CreateAttempt(_sample, _stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
