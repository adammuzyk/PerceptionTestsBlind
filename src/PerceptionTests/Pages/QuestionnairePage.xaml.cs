using System.Windows;
using System.Windows.Controls;
using PerceptionTests.Models;
using PerceptionTests.Services;
using PerceptionTests.ViewModels;

namespace PerceptionTests.Pages
{
    public partial class QuestionnairePage
    {
        private readonly bool _isMusician;
        private readonly QuestionnaireFormViewModel _viewModel;

        public QuestionnairePage(IApplicationController controller, bool isMusician)
            : base(controller, "Questionnaire")
        {
            _isMusician = isMusician;
            _viewModel = QuestionnaireFormViewModelFactory.Create(QuestionnaireCatalog.GetForm(isMusician));
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void OnSubmitClicked(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.Validate())
            {
                return;
            }

            QuestionnaireResponseMapper.MapToState(_isMusician, _viewModel.Responses, Controller.State.Questionnaire);
            Controller.NavigateTo(ApplicationStep.Session);
        }
    }
}
