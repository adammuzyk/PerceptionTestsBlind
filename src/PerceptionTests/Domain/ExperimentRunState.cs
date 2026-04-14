using System;

namespace PerceptionTests.Domain
{
    public class ExperimentRunState
    {
        public ExperimentRunState()
        {
            SessionId = Guid.NewGuid().ToString("N");
            Questionnaire = new QuestionnaireState();
            Experiment1 = new RunSessionResultGroup();
            Experiment2 = new RunSessionResultGroup();
            Experiment3 = new RunSessionResultGroup();
        }

        public string SessionId { get; }

        public QuestionnaireState Questionnaire { get; }

        public RunSessionResultGroup Experiment1 { get; }

        public RunSessionResultGroup Experiment2 { get; }

        public RunSessionResultGroup Experiment3 { get; }

        public void AddResult(Session session, RunTestResult result)
        {
            switch (session)
            {
                case Session.Test_1_1:
                    Experiment1.Session1 = result;
                    break;
                case Session.Test_1_2:
                    Experiment1.Session2 = result;
                    break;
                case Session.Test_1_3:
                    Experiment1.Session3 = result;
                    break;
                case Session.Test_2_1:
                    Experiment2.Session1 = result;
                    break;
                case Session.Test_2_2:
                    Experiment2.Session2 = result;
                    break;
                case Session.Test_2_3:
                    Experiment2.Session3 = result;
                    break;
                case Session.Test_3_1:
                    Experiment3.Session1 = result;
                    break;
                case Session.Test_3_2:
                    Experiment3.Session2 = result;
                    break;
                case Session.Test_3_3:
                    Experiment3.Session3 = result;
                    break;
            }
        }
    }
}
