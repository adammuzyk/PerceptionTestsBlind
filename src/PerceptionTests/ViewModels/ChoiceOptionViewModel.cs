namespace PerceptionTests.ViewModels
{
    public sealed class ChoiceOptionViewModel : ViewModelBase
    {
        private readonly ChoiceFieldViewModel _owner;

        public ChoiceOptionViewModel(ChoiceFieldViewModel owner, string value, string label)
        {
            _owner = owner;
            Value = value;
            Label = label;
        }

        public string Value { get; }

        public string Label { get; }

        public bool IsSelected
        {
            get { return _owner.SelectedValue == Value; }
            set
            {
                if (value)
                {
                    _owner.SelectedValue = Value;
                }
            }
        }

        internal void RaiseSelectionChanged()
        {
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
