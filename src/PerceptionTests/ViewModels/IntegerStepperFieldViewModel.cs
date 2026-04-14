using PerceptionTests.Models;

namespace PerceptionTests.ViewModels
{
    public sealed class IntegerStepperFieldViewModel : QuestionFieldViewModel
    {
        private int? _value;

        public IntegerStepperFieldViewModel(QuestionnaireFieldConfiguration configuration)
            : base(configuration)
        {
            Minimum = configuration.Min.Value;
            Maximum = configuration.Max.Value;
            Step = configuration.Step.Value;
        }

        public int Minimum { get; }

        public int Maximum { get; }

        public int Step { get; }

        public int? Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(nameof(HasValue));
                    OnPropertyChanged(nameof(RawValue));
                    ValidationError = string.Empty;
                }
            }
        }

        public string RangeDescription => "Range: " + Minimum + " to " + Maximum + ", step " + Step;

        public override bool HasValue => Value.HasValue;

        public override string RawValue => Value.HasValue
            ? Value.Value.ToString()
            : null;

        public override void ClearValue()
        {
            Value = null;
        }
    }
}
