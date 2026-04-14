using System;
using PerceptionTests.Domain;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class QuestionnaireResponseMapperTests
    {
        [Fact]
        public void MapToState_MapsMusicianResponsesAndNullsNonMusicianFields()
        {
            var responses = new QuestionnaireResponseSet();
            responses.Set(QuestionnaireFieldIds.Gender, "male");
            responses.Set(QuestionnaireFieldIds.Age, "31");
            responses.Set(QuestionnaireFieldIds.Handedness, "right");
            responses.Set(QuestionnaireFieldIds.MusicalEducationDescription, "Academy");
            responses.Set(QuestionnaireFieldIds.InstrumentLearningStartAge, "8");
            responses.Set(QuestionnaireFieldIds.InstrumentPracticeYears, "15");
            responses.Set(QuestionnaireFieldIds.HasAbsolutePitch, "True");
            responses.Set(QuestionnaireFieldIds.PrimaryPerformanceGenre, "Classical");

            var state = new QuestionnaireState();

            QuestionnaireResponseMapper.MapToState(true, responses, state);

            Assert.True(state.IsMusician);
            Assert.Equal(31, state.Age);
            Assert.True(state.HasAbsolutePitch);
            Assert.Equal("Classical", state.PrimaryPerformanceGenre);
            Assert.Null(state.HasAmateurMusicPerformanceExperience);
            Assert.Null(state.PreferredListeningMusic);
        }

        [Fact]
        public void MapToState_MapsNonMusicianResponsesAndNullsMusicianFields()
        {
            var responses = new QuestionnaireResponseSet();
            responses.Set(QuestionnaireFieldIds.Gender, "female");
            responses.Set(QuestionnaireFieldIds.Age, "22");
            responses.Set(QuestionnaireFieldIds.Handedness, "left");
            responses.Set(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, "False");
            responses.Set(QuestionnaireFieldIds.PreferredListeningMusic, "Jazz");
            responses.Set(QuestionnaireFieldIds.StudyYearAndSpecialization, "3rd year, psychology");

            var state = new QuestionnaireState();

            QuestionnaireResponseMapper.MapToState(false, responses, state);

            Assert.False(state.IsMusician);
            Assert.False(state.HasAmateurMusicPerformanceExperience);
            Assert.Equal("Jazz", state.PreferredListeningMusic);
            Assert.Null(state.MusicalEducationDescription);
            Assert.Null(state.HasAbsolutePitch);
        }

        [Fact]
        public void MapToState_ThrowsWhenRequiredIntegerIsInvalid()
        {
            var responses = new QuestionnaireResponseSet();
            responses.Set(QuestionnaireFieldIds.Gender, "female");
            responses.Set(QuestionnaireFieldIds.Age, "abc");
            responses.Set(QuestionnaireFieldIds.Handedness, "left");
            responses.Set(QuestionnaireFieldIds.HasAmateurMusicPerformanceExperience, "False");
            responses.Set(QuestionnaireFieldIds.PreferredListeningMusic, "Jazz");
            responses.Set(QuestionnaireFieldIds.StudyYearAndSpecialization, "3rd year");

            var exception = Assert.Throws<InvalidOperationException>(
                () => QuestionnaireResponseMapper.MapToState(false, responses, new QuestionnaireState()));

            Assert.Contains("not a valid integer", exception.Message);
        }
    }
}
