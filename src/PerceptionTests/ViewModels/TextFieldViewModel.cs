using PerceptionTests.Models;

namespace PerceptionTests.ViewModels
{
    public sealed class TextFieldViewModel : QuestionFieldViewModel
    {
        private string _text;

        public TextFieldViewModel(QuestionnaireFieldConfiguration configuration)
            : base(configuration)
        {
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (SetProperty(ref _text, value))
                {
                    OnPropertyChanged(nameof(RawValue));
                    ValidationError = string.Empty;
                }
            }
        }

        public override bool HasValue => !string.IsNullOrWhiteSpace(RawValue);

        public override string RawValue => string.IsNullOrWhiteSpace(Text) ? null : Text.Trim();

        public override void ClearValue()
        {
            Text = string.Empty;
        }
    }
}
