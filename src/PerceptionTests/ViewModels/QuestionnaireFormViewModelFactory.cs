using System;
using System.Collections.Generic;
using PerceptionTests.Models;

namespace PerceptionTests.ViewModels
{
    public static class QuestionnaireFormViewModelFactory
    {
        public static QuestionnaireFormViewModel Create(QuestionnaireFormConfiguration formConfiguration)
        {
            if (formConfiguration == null)
            {
                throw new ArgumentNullException(nameof(formConfiguration));
            }

            var fields = new List<QuestionFieldViewModel>();
            foreach (var fieldConfiguration in formConfiguration.Fields)
            {
                fields.Add(CreateField(fieldConfiguration));
            }

            return new QuestionnaireFormViewModel(fields);
        }

        private static QuestionFieldViewModel CreateField(QuestionnaireFieldConfiguration configuration)
        {
            switch (configuration.Type.Value)
            {
                case QuestionnaireFieldType.Choice:
                    return new ChoiceFieldViewModel(configuration);
                case QuestionnaireFieldType.Text:
                    return new TextFieldViewModel(configuration);
                case QuestionnaireFieldType.IntegerStepper:
                case QuestionnaireFieldType.IntegerSlider:
                    return new IntegerStepperFieldViewModel(configuration);
                default:
                    throw new InvalidOperationException("Unsupported questionnaire field type: " + configuration.Type);
            }
        }
    }
}
