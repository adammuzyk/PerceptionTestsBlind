using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PerceptionTests.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QuestionnaireFieldType
    {
        Choice,
        Text,
        IntegerStepper,
        IntegerSlider
    }

    public class QuestionnaireConfiguration
    {
        [JsonProperty("musician", Order = 1)]
        public QuestionnaireFormConfiguration Musician { get; set; }

        [JsonProperty("nonMusician", Order = 2)]
        public QuestionnaireFormConfiguration NonMusician { get; set; }
    }

    public class QuestionnaireFormConfiguration
    {
        [JsonProperty("version", Order = 1)]
        public string Version { get; set; }

        [JsonProperty("fields", Order = 2)]
        public IReadOnlyList<QuestionnaireFieldConfiguration> Fields { get; set; }
    }

    public class QuestionnaireFieldConfiguration
    {
        [JsonProperty("fieldId", Order = 1)]
        public string FieldId { get; set; }

        [JsonProperty("type", Order = 2)]
        public QuestionnaireFieldType? Type { get; set; }

        [JsonProperty("label", Order = 3)]
        public string Label { get; set; }

        [JsonProperty("required", Order = 4)]
        public bool Required { get; set; }

        [JsonProperty("min", Order = 5)]
        public int? Min { get; set; }

        [JsonProperty("max", Order = 6)]
        public int? Max { get; set; }

        [JsonProperty("step", Order = 7)]
        public int? Step { get; set; }

        [JsonProperty("visibleWhen", Order = 8)]
        public QuestionnaireVisibilityRuleConfiguration VisibleWhen { get; set; }

        [JsonProperty("helpText", Order = 9)]
        public string HelpText { get; set; }

        [JsonProperty("options", Order = 10)]
        public IReadOnlyList<QuestionnaireOptionConfiguration> Options { get; set; }
    }

    public class QuestionnaireVisibilityRuleConfiguration
    {
        [JsonProperty("fieldId", Order = 1)]
        public string FieldId { get; set; }

        [JsonProperty("equals", Order = 2)]
        public string EqualsValue { get; set; }
    }

    public class QuestionnaireOptionConfiguration
    {
        [JsonProperty("value", Order = 1)]
        public string Value { get; set; }

        [JsonProperty("label", Order = 2)]
        public string Label { get; set; }
    }
}
