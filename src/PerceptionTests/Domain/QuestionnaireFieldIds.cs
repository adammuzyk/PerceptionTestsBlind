namespace PerceptionTests.Domain
{
    public static class QuestionnaireFieldIds
    {
        public const string Gender = "gender";
        public const string Age = "age";
        public const string Handedness = "handedness";
        public const string MusicalEducationDescription = "musicalEducationDescription";
        public const string InstrumentLearningStartAge = "instrumentLearningStartAge";
        public const string InstrumentPracticeYears = "instrumentPracticeYears";
        public const string HasAbsolutePitch = "hasAbsolutePitch";
        public const string PrimaryPerformanceGenre = "primaryPerformanceGenre";
        public const string HasAmateurMusicPerformanceExperience = "hasAmateurMusicPerformanceExperience";
        public const string AmateurMusicActivityDetails = "amateurMusicActivityDetails";
        public const string PreferredListeningMusic = "preferredListeningMusic";
        public const string StudyYearAndSpecialization = "studyYearAndSpecialization";

        public static readonly string[] MusicianOrderedFields =
        {
            Gender,
            Age,
            Handedness,
            MusicalEducationDescription,
            InstrumentLearningStartAge,
            InstrumentPracticeYears,
            HasAbsolutePitch,
            PrimaryPerformanceGenre
        };

        public static readonly string[] NonMusicianOrderedFields =
        {
            Gender,
            Age,
            Handedness,
            HasAmateurMusicPerformanceExperience,
            AmateurMusicActivityDetails,
            PreferredListeningMusic,
            StudyYearAndSpecialization
        };
    }
}
