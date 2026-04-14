using System.Windows;
using PerceptionTests.Models;
using PerceptionTests.Services;

namespace PerceptionTests.Pages
{
    /// <summary>
    /// Interaction logic for MainQuestionPage.xaml
    /// </summary>
    public partial class MainQuestionPage
    {
        public MainQuestionPage(IApplicationController controller)
            : base(controller, "Questionnaire")
        {
            InitializeComponent();
        }

        private void OnMusicianSelected(object sender, RoutedEventArgs e)
        {
            Controller.State.Questionnaire.IsMusician = true;
            Controller.NavigateTo(ApplicationStep.MusicianQuestionnaire);
        }

        private void OnNonMusicianSelected(object sender, RoutedEventArgs e)
        {
            Controller.State.Questionnaire.IsMusician = false;
            Controller.NavigateTo(ApplicationStep.NonMusicianQuestionnaire);
        }
    }
}
