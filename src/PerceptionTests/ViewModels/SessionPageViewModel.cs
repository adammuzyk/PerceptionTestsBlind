namespace PerceptionTests.ViewModels
{
    public class SessionPageViewModel : ViewModelBase
    {
        private string _statusMessage;
        private string _actionButtonText;

        public SessionPageViewModel()
        {
            IntroText =
                "The listening task will begin when you press Start.\n" +
                "Respond once per attempt by pressing the Space key.\n\n" +
                "Press Start to begin the experiment.";
            PlaybackPrompt = "Press Space to submit your response.";
            _statusMessage = string.Empty;
            _actionButtonText = "Start";
        }

        public string IntroText { get; }

        public string PlaybackPrompt { get; }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        public string ActionButtonText
        {
            get { return _actionButtonText; }
            set { SetProperty(ref _actionButtonText, value); }
        }
    }
}
