using System.Windows;
using System.Windows.Input;
using PerceptionTests.Domain;
using PerceptionTests.Music;
using PerceptionTests.Services;
using PerceptionTests.ViewModels;

namespace PerceptionTests.Pages
{
    /// <summary>
    /// Interaction logic for SessionPage.xaml
    /// </summary>
    public partial class SessionPage
    {
        private readonly Player _player;
        private readonly Session _session;
        private readonly int _requiredValidResponses;
        private readonly SessionPageViewModel _viewModel;
        private bool _end;
        private readonly RunTestResult _result;

        public SessionPage(IApplicationController controller, Session session)
            : base(controller)
        {
            _session = session;
            _requiredValidResponses = ExperimentCatalog.GetSessionConfiguration(session).RequiredValidResponses;
            _viewModel = new SessionPageViewModel();
            _player = new Player(new SampleGenerator(), new WaveGenerator(controller.RuntimeSettings));
            _player.PrepareSession(session);
            _result = new RunTestResult();
            _end = false;

            InitializeComponent();
            DataContext = _viewModel;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_end)
            {
                SaveResult();
                Controller.NavigateToNextTest(_session);
            }
            else
            {
                Controller.PreviewKeyDown += OnKeyDown;
                MainTabControl.SelectedItem = SecondTabItem;
                _player.Play();
            }
        }

        private void SaveResult()
        {
            Controller.State.AddResult(_session, _result);
            var checkpointResult = Controller.SaveCheckpoint();
            if (!checkpointResult.Success)
            {
                MessageBox.Show(
                    "Checkpoint save failed.\n\n" + checkpointResult.ErrorMessage,
                    "Save Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            Controller.PreviewKeyDown -= OnKeyDown;
            MainTabControl.SelectedItem = ThirdTabItem;
            var attempt = _player.Stop();
            _result.AddAttempt(attempt);

            if (!attempt.MappedToneDurationMilliseconds.HasValue)
            {
                _viewModel.StatusMessage = "No response was captured during playback. The attempt will be repeated.";
            }
            else
            {
                _viewModel.StatusMessage = "Press Start to continue to the next attempt.";
                if (_result.ValidResponseCount == _requiredValidResponses)
                {
                    _viewModel.StatusMessage = "Press Next to move to the next experiment.";
                    _viewModel.ActionButtonText = "Next";
                    _end = true;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                StopButton_OnClick(null, null);
                e.Handled = true;
            }
        }
    }
}
