using System.Collections.Generic;
using System.Linq;
using PerceptionTests.Models;

namespace PerceptionTests.ViewModels
{
    public sealed class ChoiceFieldViewModel : QuestionFieldViewModel
    {
        private string _selectedValue;

        public ChoiceFieldViewModel(QuestionnaireFieldConfiguration configuration)
            : base(configuration)
        {
            Options = configuration.Options ?? new QuestionnaireOptionConfiguration[0];
            OptionItems = Options
                .Select(option => new ChoiceOptionViewModel(this, option.Value, option.Label))
                .ToArray();
        }

        public IReadOnlyList<QuestionnaireOptionConfiguration> Options { get; }

        public IReadOnlyList<ChoiceOptionViewModel> OptionItems { get; }

        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (SetProperty(ref _selectedValue, value))
                {
                    OnPropertyChanged(nameof(RawValue));
                    foreach (var option in OptionItems)
                    {
                        option.RaiseSelectionChanged();
                    }
                    ValidationError = string.Empty;
                }
            }
        }

        public override bool HasValue => !string.IsNullOrWhiteSpace(SelectedValue);

        public override string RawValue => SelectedValue;

        public override void ClearValue()
        {
            SelectedValue = null;
        }
    }
}
