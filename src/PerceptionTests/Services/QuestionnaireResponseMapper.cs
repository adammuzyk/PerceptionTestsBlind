using System;
using System.Globalization;
using PerceptionTests.Domain;

namespace PerceptionTests.Services
{
    public static class QuestionnaireResponseMapper
    {
        public static void MapToState(bool isMusician, QuestionnaireResponseSet responses, QuestionnaireState state)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            state.IsMusician = isMusician;
            state.Gender = ParseRequiredString(responses, QuestionnaireFieldIds.Gender);
            state.Age = ParseRequiredInt(responses, QuestionnaireFieldIds.Age);
            state.Handedness = ParseRequiredString(responses, QuestionnaireFieldIds.Handedness);

            if (isMusician)
            {
                state.MusicalEducationDescription = ParseRequiredString(responses, QuestionnaireFieldIds.MusicalEducationDescription);
                state.InstrumentLearningStartAge = ParseRequiredInt(responses, QuestionnaireFieldIds.InstrumentLearningStartAge);
                state.InstrumentPracticeYears = ParseRequiredInt(responses, QuestionnaireFieldIds.InstrumentPracticeYears);
                state.HasAbsolutePitch = ParseRequiredBool(responses, QuestionnaireFieldIds.HasAbsolutePitch);
                state.PrimaryPerformanceGenre = ParseRequiredString(responses, QuestionnaireFieldIds.PrimaryPerformanceGenre);

                state.HasAmateurMusicPerformanceExperience = null;
                state.AmateurMusicActivityDetails = null;
                state.PreferredListeningMusic = null;
                state.StudyYearAndSpecialization = null;
                return;
            }

            state.HasAmateurMusicPerformanceExperience = ParseRequiredBool(responses, QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience);
            state.AmateurMusicActivityDetails = ParseNullableString(responses, QuestionnaireFieldIds.AmateurMusicActivityDetails);
            state.PreferredListeningMusic = ParseRequiredString(responses, QuestionnaireFieldIds.PreferredListeningMusic);
            state.StudyYearAndSpecialization = ParseRequiredString(responses, QuestionnaireFieldIds.StudyYearAndSpecialization);

            state.MusicalEducationDescription = null;
            state.InstrumentLearningStartAge = null;
            state.InstrumentPracticeYears = null;
            state.HasAbsolutePitch = null;
            state.PrimaryPerformanceGenre = null;
        }

        internal static string ParseRequiredString(QuestionnaireResponseSet responses, string fieldId)
        {
            var value = ParseNullableString(responses, fieldId);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Questionnaire response '" + fieldId + "' is required.");
            }

            return value;
        }

        internal static string ParseNullableString(QuestionnaireResponseSet responses, string fieldId)
        {
            return responses.Get(fieldId)?.Trim();
        }

        internal static int? ParseNullableInt(QuestionnaireResponseSet responses, string fieldId)
        {
            var value = ParseNullableString(responses, fieldId);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                throw new InvalidOperationException("Questionnaire response '" + fieldId + "' is not a valid integer.");
            }

            return parsed;
        }

        internal static int ParseRequiredInt(QuestionnaireResponseSet responses, string fieldId)
        {
            var value = ParseNullableInt(responses, fieldId);
            if (!value.HasValue)
            {
                throw new InvalidOperationException("Questionnaire response '" + fieldId + "' is required.");
            }

            return value.Value;
        }

        internal static bool? ParseNullableBool(QuestionnaireResponseSet responses, string fieldId)
        {
            var value = ParseNullableString(responses, fieldId);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (!bool.TryParse(value, out var parsed))
            {
                throw new InvalidOperationException("Questionnaire response '" + fieldId + "' is not a valid boolean.");
            }

            return parsed;
        }

        internal static bool ParseRequiredBool(QuestionnaireResponseSet responses, string fieldId)
        {
            var value = ParseNullableBool(responses, fieldId);
            if (!value.HasValue)
            {
                throw new InvalidOperationException("Questionnaire response '" + fieldId + "' is required.");
            }

            return value.Value;
        }
    }
}
