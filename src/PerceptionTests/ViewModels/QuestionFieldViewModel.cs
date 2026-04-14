using PerceptionTests.Models;

namespace PerceptionTests.ViewModels
{
    public abstract class QuestionFieldViewModel : ViewModelBase
    {
        private bool _isVisible = true;
        private string _validationError;

        protected QuestionFieldViewModel(QuestionnaireFieldConfiguration configuration)
        {
            Configuration = configuration;
            FieldId = configuration.FieldId;
            Label = configuration.Label;
            IsRequired = configuration.Required;
            HelpText = configuration.HelpText;
            FieldType = configuration.Type.Value;
            VisibleWhen = configuration.VisibleWhen;
        }

        public QuestionnaireFieldConfiguration Configuration { get; }

        public string FieldId { get; }

        public string Label { get; }

        public bool IsRequired { get; }

        public string HelpText { get; }

        public QuestionnaireFieldType FieldType { get; }

        public QuestionnaireVisibilityRuleConfiguration VisibleWhen { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public string ValidationError
        {
            get { return _validationError; }
            set { SetProperty(ref _validationError, value); }
        }

        public abstract bool HasValue { get; }

        public abstract string RawValue { get; }

        public abstract void ClearValue();
    }
}
