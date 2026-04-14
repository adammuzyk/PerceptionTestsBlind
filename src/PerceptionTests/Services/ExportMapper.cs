using System;
using PerceptionTests.Domain;
using PerceptionTests.Models;

namespace PerceptionTests.Services
{
    public static class ExportMapper
    {
        public static TestModel Map(ExperimentRunState state, DateTime exportedAtUtc)
        {
            return new TestModel
            {
                Metadata = ExportMetadata.CreateForRun(
                    state.SessionId,
                    exportedAtUtc,
                    QuestionnaireCatalog.GetFormVersion(state.Questionnaire.IsMusician)),
                Questionnaire = MapQuestionnaire(state.Questionnaire),
                Experiment1 = MapSessionGroup(state.Experiment1),
                Experiment2 = MapSessionGroup(state.Experiment2),
                Experiment3 = MapSessionGroup(state.Experiment3)
            };
        }

        private static QuestionnaireData MapQuestionnaire(QuestionnaireState state)
        {
            return new QuestionnaireData
            {
                IsMusician = state.IsMusician,
                Gender = state.Gender,
                Age = state.Age,
                Handedness = state.Handedness,
                MusicalEducationDescription = state.MusicalEducationDescription,
                InstrumentLearningStartAge = state.InstrumentLearningStartAge,
                InstrumentPracticeYears = state.InstrumentPracticeYears,
                HasAbsolutePitch = state.HasAbsolutePitch,
                PrimaryPerformanceGenre = state.PrimaryPerformanceGenre,
                HasAmateurMusicPerformanceExperience = state.HasAmateurMusicPerformanceExperience,
                AmateurMusicActivityDetails = state.AmateurMusicActivityDetails,
                PreferredListeningMusic = state.PreferredListeningMusic,
                StudyYearAndSpecialization = state.StudyYearAndSpecialization
            };
        }

        private static SessionResult MapSessionGroup(RunSessionResultGroup group)
        {
            return new SessionResult
            {
                Session1 = MapTestResult(group.Session1),
                Session2 = MapTestResult(group.Session2),
                Session3 = MapTestResult(group.Session3)
            };
        }

        private static TestResult MapTestResult(RunTestResult source)
        {
            if (source == null)
            {
                return null;
            }

            var export = new TestResult();
            foreach (var attempt in source.Attempts)
            {
                export.Attempts.Add(new ResponseAttempt
                {
                    AttemptNumber = attempt.AttemptNumber,
                    RawResponseTimeMilliseconds = attempt.RawResponseTimeMilliseconds,
                    MappedToneDurationMilliseconds = attempt.MappedToneDurationMilliseconds
                });
            }

            return export;
        }
    }
}
