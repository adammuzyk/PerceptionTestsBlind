using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PerceptionTests.Domain;
using Newtonsoft.Json;
using PerceptionTests.Models;

namespace PerceptionTests.Services
{
    public static class QuestionnaireCatalog
    {
        private static QuestionnaireConfiguration _configuration;
        private static IReadOnlyDictionary<string, QuestionnaireFieldConfiguration> _musicianFields;
        private static IReadOnlyDictionary<string, QuestionnaireFieldConfiguration> _nonMusicianFields;

        public static void Initialize(string configurationPath)
        {
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                throw new InvalidOperationException("Questionnaire configuration path is missing.");
            }

            var configuration = LoadConfigurationFromFile(configurationPath);
            ValidateConfiguration(configuration);

            _configuration = configuration;
            _musicianFields = BuildFieldLookup(configuration.Musician);
            _nonMusicianFields = BuildFieldLookup(configuration.NonMusician);
        }

        public static QuestionnaireFieldConfiguration GetMusicianField(string fieldId)
        {
            EnsureInitialized();
            return GetField(_musicianFields, fieldId, "musician");
        }

        public static QuestionnaireFieldConfiguration GetNonMusicianField(string fieldId)
        {
            EnsureInitialized();
            return GetField(_nonMusicianFields, fieldId, "nonMusician");
        }

        internal static QuestionnaireConfiguration LoadConfigurationFromFile(string configurationPath)
        {
            if (!File.Exists(configurationPath))
            {
                throw new InvalidOperationException("Questionnaire configuration file was not found: " + configurationPath);
            }

            var json = File.ReadAllText(configurationPath);
            var configuration = JsonConvert.DeserializeObject<QuestionnaireConfiguration>(json);
            if (configuration == null)
            {
                throw new InvalidOperationException("Questionnaire configuration file is empty or invalid JSON: " + configurationPath);
            }

            return configuration;
        }

        internal static void ValidateConfiguration(QuestionnaireConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Questionnaire configuration is not initialized.");
            }

            ValidateForm("musician", configuration.Musician, MusicianFieldIds);
            ValidateForm("nonMusician", configuration.NonMusician, NonMusicianFieldIds);
        }

        private static void ValidateForm(string formName, QuestionnaireFormConfiguration form, IReadOnlyCollection<string> expectedFieldIds)
        {
            if (form == null || form.Fields == null)
            {
                throw new InvalidOperationException("Questionnaire configuration section '" + formName + "' is missing.");
            }

            if (string.IsNullOrWhiteSpace(form.Version))
            {
                throw new InvalidOperationException("Questionnaire configuration section '" + formName + "' must define a version.");
            }

            var actualFieldIds = form.Fields.Select(field => field.FieldId).ToList();
            if (actualFieldIds.Count != actualFieldIds.Distinct(StringComparer.Ordinal).Count())
            {
                throw new InvalidOperationException("Questionnaire configuration section '" + formName + "' contains duplicate fieldId values.");
            }

            if (!actualFieldIds.SequenceEqual(expectedFieldIds))
            {
                throw new InvalidOperationException("Questionnaire configuration section '" + formName + "' does not define the expected ordered field set.");
            }

            foreach (var field in form.Fields)
            {
                ValidateField(formName, field, actualFieldIds);
            }
        }

        public static QuestionnaireFormConfiguration GetForm(bool isMusician)
        {
            EnsureInitialized();
            return isMusician ? _configuration.Musician : _configuration.NonMusician;
        }

        public static string GetFormVersion(bool isMusician)
        {
            return GetForm(isMusician).Version;
        }

        private static void ValidateField(
            string formName,
            QuestionnaireFieldConfiguration field,
            IReadOnlyCollection<string> availableFieldIds)
        {
            if (field == null)
            {
                throw new InvalidOperationException("Questionnaire field configuration in '" + formName + "' is null.");
            }

            if (string.IsNullOrWhiteSpace(field.FieldId))
            {
                throw new InvalidOperationException("Questionnaire configuration section '" + formName + "' contains a field without fieldId.");
            }

            if (!field.Type.HasValue)
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' must define a type.");
            }

            if (string.IsNullOrWhiteSpace(field.Label))
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' must define a label.");
            }

            ValidateVisibilityRule(formName, field, availableFieldIds);

            switch (field.Type.Value)
            {
                case QuestionnaireFieldType.Choice:
                    ValidateChoiceField(formName, field);
                    return;
                case QuestionnaireFieldType.Text:
                    ValidateTextField(formName, field);
                    return;
                case QuestionnaireFieldType.IntegerStepper:
                case QuestionnaireFieldType.IntegerSlider:
                    ValidateIntegerField(formName, field);
                    return;
            }
        }

        private static void ValidateChoiceField(string formName, QuestionnaireFieldConfiguration field)
        {
            if (field.Options == null || field.Options.Count == 0)
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' defines an empty options list.");
            }

            foreach (var option in field.Options)
            {
                if (option == null || string.IsNullOrWhiteSpace(option.Value) || string.IsNullOrWhiteSpace(option.Label))
                {
                    throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' contains an invalid option.");
                }
            }

            if (field.Min.HasValue || field.Max.HasValue || field.Step.HasValue)
            {
                throw new InvalidOperationException("Choice field '" + field.FieldId + "' in '" + formName + "' cannot define slider range metadata.");
            }
        }

        private static void ValidateTextField(string formName, QuestionnaireFieldConfiguration field)
        {
            if (field.Options != null && field.Options.Count > 0)
            {
                throw new InvalidOperationException("Text field '" + field.FieldId + "' in '" + formName + "' cannot define choice options.");
            }

            if (field.Min.HasValue || field.Max.HasValue || field.Step.HasValue)
            {
                throw new InvalidOperationException("Text field '" + field.FieldId + "' in '" + formName + "' cannot define slider range metadata.");
            }
        }

        private static void ValidateIntegerField(string formName, QuestionnaireFieldConfiguration field)
        {
            if (field.Options != null && field.Options.Count > 0)
            {
                throw new InvalidOperationException("Integer field '" + field.FieldId + "' in '" + formName + "' cannot define choice options.");
            }

            if (!field.Min.HasValue || !field.Max.HasValue || !field.Step.HasValue)
            {
                throw new InvalidOperationException("Integer field '" + field.FieldId + "' in '" + formName + "' must define min, max, and step.");
            }

            if (field.Max.Value < field.Min.Value)
            {
                throw new InvalidOperationException("Integer field '" + field.FieldId + "' in '" + formName + "' must define max greater than or equal to min.");
            }

            if (field.Step.Value <= 0)
            {
                throw new InvalidOperationException("Integer field '" + field.FieldId + "' in '" + formName + "' must define step greater than zero.");
            }
        }

        private static void ValidateVisibilityRule(
            string formName,
            QuestionnaireFieldConfiguration field,
            IReadOnlyCollection<string> availableFieldIds)
        {
            var visibleWhen = field.VisibleWhen;
            if (visibleWhen == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(visibleWhen.FieldId) || string.IsNullOrWhiteSpace(visibleWhen.EqualsValue))
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' defines an invalid visibleWhen rule.");
            }

            if (string.Equals(field.FieldId, visibleWhen.FieldId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' cannot reference itself in visibleWhen.");
            }

            if (!availableFieldIds.Contains(visibleWhen.FieldId, StringComparer.Ordinal))
            {
                throw new InvalidOperationException("Questionnaire field '" + field.FieldId + "' in '" + formName + "' references unknown visibleWhen field '" + visibleWhen.FieldId + "'.");
            }
        }

        private static IReadOnlyDictionary<string, QuestionnaireFieldConfiguration> BuildFieldLookup(QuestionnaireFormConfiguration form)
        {
            return form.Fields.ToDictionary(field => field.FieldId, StringComparer.Ordinal);
        }

        private static QuestionnaireFieldConfiguration GetField(
            IReadOnlyDictionary<string, QuestionnaireFieldConfiguration> fieldLookup,
            string fieldId,
            string formName)
        {
            if (!fieldLookup.TryGetValue(fieldId, out var field))
            {
                throw new InvalidOperationException("Questionnaire field '" + fieldId + "' is missing in '" + formName + "' configuration.");
            }

            return field;
        }

        private static void EnsureInitialized()
        {
            if (_configuration == null || _musicianFields == null || _nonMusicianFields == null)
            {
                throw new InvalidOperationException("Questionnaire catalog has not been initialized.");
            }
        }

        private static IReadOnlyCollection<string> MusicianFieldIds => QuestionnaireFieldIds.MusicianOrderedFields;

        private static IReadOnlyCollection<string> NonMusicianFieldIds => QuestionnaireFieldIds.NonMusicianOrderedFields;
    }
}
