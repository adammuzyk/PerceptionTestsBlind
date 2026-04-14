using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class JsonExportTests
    {
        [Fact]
        [Trait("Suite", "Contract")]
        public void Export_UsesStructuredStableSchemaAndIncludesMetadata()
        {
            ExperimentCatalogTestHelper.EnsureInitialized();
            var state = new ExperimentRunState();
            state.Questionnaire.IsMusician = true;
            state.Questionnaire.Gender = "male";
            state.Questionnaire.Age = 24;
            state.Questionnaire.Handedness = "right";
            state.Questionnaire.HasAbsolutePitch = false;

            var testResult = new RunTestResult();
            testResult.AddAttempt(new RunResponseAttempt
            {
                RawResponseTimeMilliseconds = 123.45,
                MappedToneDurationMilliseconds = 80
            });
            state.AddResult(Session.Test_1_1, testResult);

            var model = ExportMapper.Map(state, new DateTime(2026, 3, 22, 12, 30, 0, DateTimeKind.Utc));

            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var root = JObject.Parse(json);
            var expectedQuestionnaireVersion = QuestionnaireCatalog.GetFormVersion(true);

            Assert.NotNull(root["metadata"]);
            Assert.NotNull(root["metadata"]["applicationVersion"]);
            Assert.Equal(expectedQuestionnaireVersion, (string)root["metadata"]["questionnaireVersion"]);
            Assert.Equal("male", (string)root["questionnaire"]["gender"]);
            Assert.Equal(24, (int)root["questionnaire"]["age"]);
            Assert.Equal("right", (string)root["questionnaire"]["handedness"]);
            Assert.NotNull(root["metadata"]["experimentConfiguration"]);
            Assert.NotNull(root["Experiment1"]["Session1"]["attempts"][0]["rawResponseTimeMilliseconds"]);
            Assert.DoesNotContain("Are you a musician?", json);
        }
    }
}
