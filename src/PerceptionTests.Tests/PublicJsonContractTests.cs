using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.Services;
using Xunit;

namespace PerceptionTests.Tests
{
    public class PublicJsonContractTests
    {
        [Fact]
        [Trait("Suite", "Contract")]
        public void ExportedResult_ContainsRequiredPublicContractFields()
        {
            ExperimentCatalogTestHelper.EnsureInitialized();

            var state = new ExperimentRunState();
            state.Questionnaire.IsMusician = true;
            state.Questionnaire.Gender = "female";
            state.Questionnaire.Age = 27;
            state.Questionnaire.Handedness = "right";
            state.Questionnaire.PrimaryPerformanceGenre = "Classical";

            var result = new RunTestResult();
            result.AddAttempt(new RunResponseAttempt
            {
                RawResponseTimeMilliseconds = 215.4,
                MappedToneDurationMilliseconds = 80
            });
            state.AddResult(Session.Test_1_1, result);

            var export = ExportMapper.Map(state, new DateTime(2026, 4, 14, 10, 0, 0, DateTimeKind.Utc));
            var json = JsonConvert.SerializeObject(export, Formatting.Indented);
            var root = JObject.Parse(json);

            Assert.NotNull(root["metadata"]);
            Assert.NotNull(root["questionnaire"]);
            Assert.NotNull(root["Experiment1"]);
            Assert.NotNull(root["Experiment2"]);
            Assert.NotNull(root["Experiment3"]);

            Assert.NotNull(root["metadata"]["applicationVersion"]);
            Assert.NotNull(root["metadata"]["exportedAtUtc"]);
            Assert.NotNull(root["metadata"]["sessionId"]);
            Assert.NotNull(root["metadata"]["questionnaireVersion"]);
            Assert.NotNull(root["metadata"]["experimentConfiguration"]);

            var attempt = root["Experiment1"]["Session1"]["attempts"][0];
            Assert.NotNull(attempt["attemptNumber"]);
            Assert.NotNull(attempt["rawResponseTimeMilliseconds"]);
            Assert.NotNull(attempt["mappedToneDurationMilliseconds"]);
            Assert.NotNull(attempt["responseCapturedWithinStimulus"]);
        }
    }
}
