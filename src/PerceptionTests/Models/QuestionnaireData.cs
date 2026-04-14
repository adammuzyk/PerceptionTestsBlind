using Newtonsoft.Json;

namespace PerceptionTests.Models
{
    public class QuestionnaireData
    {
        [JsonProperty("isMusician", Order = 1)]
        public bool IsMusician { get; set; }

        [JsonProperty("gender", Order = 2)]
        public string Gender { get; set; }

        [JsonProperty("age", Order = 3)]
        public int? Age { get; set; }

        [JsonProperty("handedness", Order = 4)]
        public string Handedness { get; set; }

        [JsonProperty("musicalEducationDescription", Order = 5)]
        public string MusicalEducationDescription { get; set; }

        [JsonProperty("instrumentLearningStartAge", Order = 6)]
        public int? InstrumentLearningStartAge { get; set; }

        [JsonProperty("instrumentPracticeYears", Order = 7)]
        public int? InstrumentPracticeYears { get; set; }

        [JsonProperty("hasAbsolutePitch", Order = 8)]
        public bool? HasAbsolutePitch { get; set; }

        [JsonProperty("primaryPerformanceGenre", Order = 9)]
        public string PrimaryPerformanceGenre { get; set; }

        [JsonProperty("hasAmateurMusicPerformanceExperience", Order = 10)]
        public bool? HasAmateurMusicPerformanceExperience { get; set; }

        [JsonProperty("amateurMusicActivityDetails", Order = 11)]
        public string AmateurMusicActivityDetails { get; set; }

        [JsonProperty("preferredListeningMusic", Order = 12)]
        public string PreferredListeningMusic { get; set; }

        [JsonProperty("studyYearAndSpecialization", Order = 13)]
        public string StudyYearAndSpecialization { get; set; }
    }
}
