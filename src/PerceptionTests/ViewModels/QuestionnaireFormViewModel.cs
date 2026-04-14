using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using PerceptionTests.Domain;

namespace PerceptionTests.ViewModels
{
    public sealed class QuestionnaireFormViewModel : ViewModelBase
    {
        private bool _isUpdatingVisibility;
        private string _validationMessage;

        public QuestionnaireFormViewModel(IEnumerable<QuestionFieldViewModel> fields)
        {
            Fields = new ObservableCollection<QuestionFieldViewModel>(fields);
            Responses = new QuestionnaireResponseSet();

            foreach (var field in Fields)
            {
                field.PropertyChanged += FieldOnPropertyChanged;
            }

            ReevaluateVisibility();
        }

        public ObservableCollection<QuestionFieldViewModel> Fields { get; }

        public QuestionnaireResponseSet Responses { get; }

        public string ValidationMessage
        {
            get { return _validationMessage; }
            set { SetProperty(ref _validationMessage, value); }
        }

        public bool Validate()
        {
            var hasErrors = false;

            foreach (var field in Fields)
            {
                field.ValidationError = string.Empty;

                if (!field.IsVisible)
                {
                    continue;
                }

                if (field.IsRequired && !field.HasValue)
                {
                    field.ValidationError = "This field is required.";
                    hasErrors = true;
                    continue;
                }

            }

            ValidationMessage = hasErrors
                ? "Please complete all required fields."
                : string.Empty;

            return !hasErrors;
        }

        public void ReevaluateVisibility()
        {
            _isUpdatingVisibility = true;

            try
            {
                var didChange = true;
                while (didChange)
                {
                    didChange = false;

                    foreach (var field in Fields)
                    {
                        var shouldBeVisible = EvaluateVisibility(field);
                        if (field.IsVisible != shouldBeVisible)
                        {
                            field.IsVisible = shouldBeVisible;
                            didChange = true;
                        }

                        if (!shouldBeVisible && field.HasValue)
                        {
                            field.ClearValue();
                            field.ValidationError = string.Empty;
                            didChange = true;
                        }
                    }
                }
            }
            finally
            {
                _isUpdatingVisibility = false;
            }

            SynchronizeResponses();
        }

        private bool EvaluateVisibility(QuestionFieldViewModel field)
        {
            if (field.VisibleWhen == null)
            {
                return true;
            }

            var controllingField = Fields.First(item => item.FieldId == field.VisibleWhen.FieldId);
            return string.Equals(controllingField.RawValue, field.VisibleWhen.EqualsValue, StringComparison.Ordinal);
        }

        private void FieldOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(QuestionFieldViewModel.RawValue))
            {
                return;
            }

            var field = sender as QuestionFieldViewModel;
            if (field != null && !string.IsNullOrWhiteSpace(field.ValidationError))
            {
                field.ValidationError = string.Empty;
            }

            if (!_isUpdatingVisibility)
            {
                ReevaluateVisibility();
            }

            SynchronizeResponses();

            if (Fields.Where(item => item.IsVisible && item.IsRequired).All(item => item.HasValue))
            {
                ValidationMessage = string.Empty;
            }
        }

        private void SynchronizeResponses()
        {
            Responses.Clear();

            foreach (var field in Fields)
            {
                if (!field.IsVisible || !field.HasValue)
                {
                    continue;
                }

                Responses.Set(field.FieldId, field.RawValue);
            }
        }
    }
}
