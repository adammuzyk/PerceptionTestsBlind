using System.Windows.Input;
using PerceptionTests.Domain;
using PerceptionTests.Models;

namespace PerceptionTests.Services
{
    public interface IApplicationController
    {
        ExperimentRunState State { get; }

        RuntimeSettings RuntimeSettings { get; }

        string WindowTitle { set; }

        event KeyEventHandler PreviewKeyDown;

        void NavigateTo(ApplicationStep nextStep);

        void NavigateToNextTest(Session fromTest);

        PersistenceResult SaveCheckpoint();

        PersistenceResult SaveFinalResult();

        void CloseApplication();
    }
}
