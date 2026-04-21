using System.Linq;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.ViewModels;
using Xunit;

namespace PerceptionTests.Tests
{
    public class QuestionnaireFormViewModelTests
    {
        private const string TestVersion = "test-version";

        [Fact]
        public void Factory_CreatesExpectedFieldViewModelTypes()
        {
            var form = new QuestionnaireFormConfiguration
            {
                Version = TestVersion,
                Fields = new QuestionnaireFieldConfiguration[]
                {
                    Choice(QuestionnaireFieldIds.Gender, "Gender"),
                    Stepper(QuestionnaireFieldIds.Age, "Age"),
                    Text(QuestionnaireFieldIds.PreferredListeningMusic, "Preferred music")
                }
            };

            var viewModel = QuestionnaireFormViewModelFactory.Create(form);

            Assert.IsType<ChoiceFieldViewModel>(viewModel.Fields[0]);
            Assert.IsType<IntegerStepperFieldViewModel>(viewModel.Fields[1]);
            Assert.IsType<TextFieldViewModel>(viewModel.Fields[2]);
        }

        [Fact]
        public void Validate_RequiresVisibleFieldsOnly()
        {
            var viewModel = QuestionnaireFormViewModelFactory.Create(CreateConditionalForm());

            var isValid = viewModel.Validate();

            Assert.False(isValid);
            Assert.Equal("Please complete all required fields.", viewModel.ValidationMessage);
            Assert.Equal("This field is required.", viewModel.Fields[0].ValidationError);
            Assert.True(string.IsNullOrEmpty(viewModel.Fields[1].ValidationError));
        }

        [Fact]
        public void FieldChange_ClearsFieldValidationMessage()
        {
            var viewModel = QuestionnaireFormViewModelFactory.Create(CreateConditionalForm());
            var choiceField = (ChoiceFieldViewModel)viewModel.Fields[0];

            Assert.False(viewModel.Validate());

            choiceField.SelectedValue = "True";

            Assert.True(string.IsNullOrEmpty(choiceField.ValidationError));
            Assert.Equal("Please complete all required fields.", viewModel.ValidationMessage);
        }

        [Fact]
        public void FieldChange_ClearsFormValidationMessageWhenAllVisibleRequiredFieldsHaveValues()
        {
            var viewModel = QuestionnaireFormViewModelFactory.Create(CreateConditionalForm());
            var choiceField = (ChoiceFieldViewModel)viewModel.Fields[0];
            var detailsField = (TextFieldViewModel)viewModel.Fields[1];

            Assert.False(viewModel.Validate());

            choiceField.SelectedValue = "True";
            detailsField.Text = "Choir for 4 years";

            Assert.True(string.IsNullOrEmpty(choiceField.ValidationError));
            Assert.True(string.IsNullOrEmpty(detailsField.ValidationError));
            Assert.True(string.IsNullOrEmpty(viewModel.ValidationMessage));
        }

        [Fact]
        public void ReevaluateVisibility_ClearsHiddenDependentValueAndResponse()
        {
            var viewModel = QuestionnaireFormViewModelFactory.Create(CreateConditionalForm());
            var choiceField = (ChoiceFieldViewModel)viewModel.Fields[0];
            var detailsField = (TextFieldViewModel)viewModel.Fields[1];

            choiceField.SelectedValue = "True";
            Assert.True(detailsField.IsVisible);

            detailsField.Text = "Choir for 4 years";
            Assert.Equal("Choir for 4 years", viewModel.Responses.Get(QuestionnaireFieldIds.AmateurMusicActivityDetails));

            choiceField.SelectedValue = "False";

            Assert.False(detailsField.IsVisible);
            Assert.True(string.IsNullOrWhiteSpace(detailsField.Text));
            Assert.False(viewModel.Responses.Contains(QuestionnaireFieldIds.AmateurMusicActivityDetails));
        }

        [Fact]
        public void IntegerStepperField_UsesButtonsAndTypedInput()
        {
            var form = new QuestionnaireFormConfiguration
            {
                Version = TestVersion,
                Fields = new QuestionnaireFieldConfiguration[]
                {
                    Stepper(QuestionnaireFieldIds.Age, "Age")
                }
            };

            var viewModel = QuestionnaireFormViewModelFactory.Create(form);
            var field = (IntegerStepperFieldViewModel)viewModel.Fields.Single();

            Assert.False(field.HasValue);
            Assert.Equal(21, field.DefaultValue);

            field.Value = 1;

            Assert.True(field.HasValue);
            Assert.Equal("1", field.RawValue);

            field.Value = 11;

            Assert.True(field.HasValue);
            Assert.Equal("11", field.RawValue);
        }

        [Fact]
        public void IntegerStepperField_ClearsValue()
        {
            var form = new QuestionnaireFormConfiguration
            {
                Version = TestVersion,
                Fields = new QuestionnaireFieldConfiguration[]
                {
                    Stepper(QuestionnaireFieldIds.Age, "Age")
                }
            };

            var viewModel = QuestionnaireFormViewModelFactory.Create(form);
            var field = (IntegerStepperFieldViewModel)viewModel.Fields.Single();

            field.Value = 10;
            field.ClearValue();

            Assert.False(field.HasValue);
            Assert.Null(field.RawValue);
        }

        [Fact]
        public void IntegerStepperField_UsesMinimumAsDefaultForNonAgeFields()
        {
            var form = new QuestionnaireFormConfiguration
            {
                Version = TestVersion,
                Fields = new QuestionnaireFieldConfiguration[]
                {
                    Stepper(QuestionnaireFieldIds.InstrumentPracticeYears, "Practice years")
                }
            };

            var viewModel = QuestionnaireFormViewModelFactory.Create(form);
            var field = (IntegerStepperFieldViewModel)viewModel.Fields.Single();

            Assert.Equal(1, field.DefaultValue);
        }

        private static QuestionnaireFormConfiguration CreateConditionalForm()
        {
            return new QuestionnaireFormConfiguration
            {
                Version = TestVersion,
                Fields = new QuestionnaireFieldConfiguration[]
                {
                    Choice(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, "Have you ever taken part in amateur music performance?"),
                    Text(
                        QuestionnaireFieldIds.AmateurMusicActivityDetails,
                        "Please describe the musical activity and how long you took part in it.",
                        new QuestionnaireVisibilityRuleConfiguration
                        {
                            FieldId = QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience,
                            EqualsValue = "True"
                        })
                }
            };
        }

        private static QuestionnaireFieldConfiguration Choice(string fieldId, string label)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.Choice,
                Label = label,
                Required = true,
                Options = new[]
                {
                    new QuestionnaireOptionConfiguration { Value = "True", Label = "Yes" },
                    new QuestionnaireOptionConfiguration { Value = "False", Label = "No" }
                }
            };
        }

        private static QuestionnaireFieldConfiguration Text(
            string fieldId,
            string label,
            QuestionnaireVisibilityRuleConfiguration visibleWhen = null)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.Text,
                Label = label,
                Required = true,
                VisibleWhen = visibleWhen
            };
        }

        private static QuestionnaireFieldConfiguration Stepper(string fieldId, string label)
        {
            return new QuestionnaireFieldConfiguration
            {
                FieldId = fieldId,
                Type = QuestionnaireFieldType.IntegerStepper,
                Label = label,
                Required = true,
                Min = 1,
                Max = 100,
                Step = 1
            };
        }
    }
}
