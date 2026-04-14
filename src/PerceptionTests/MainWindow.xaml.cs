using System;
using System.Windows;
using PerceptionTests.Domain;
using PerceptionTests.Models;
using PerceptionTests.Music;
using PerceptionTests.Pages;
using PerceptionTests.Services;

namespace PerceptionTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IApplicationController
    {
        private readonly ResultPersistenceService _persistenceService;

        public MainWindow()
        {
            try
            {
                RuntimeSettings = RuntimeSettings.Load();
                ExperimentCatalog.Initialize(RuntimeSettings.ExperimentConfigurationPath);
                QuestionnaireCatalog.Initialize(RuntimeSettings.QuestionnaireConfigurationPath);
                _persistenceService = new ResultPersistenceService(RuntimeSettings);
                State = new ExperimentRunState();

                InitializeComponent();
                NavigateTo(ApplicationStep.Welcome);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Application configuration is invalid.\n\n" + ex.Message,
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
        }

        public ExperimentRunState State { get; private set; }

        public RuntimeSettings RuntimeSettings { get; private set; }

        public string WindowTitle
        {
            set { Title = value; }
        }

        public void NavigateTo(ApplicationStep nextStep)
        {
            ContentControl.Content = CreateViewForStep(nextStep);
        }

        public void NavigateToNextTest(Session fromTest)
        {
            if (fromTest == Session.Test_3_3)
            {
                NavigateTo(ApplicationStep.ThankYou);
            }
            else
            {
                ContentControl.Content = new SessionPage(this, fromTest + 1);
            }
        }

        public PersistenceResult SaveCheckpoint()
        {
            return _persistenceService.SaveCheckpoint(State);
        }

        public PersistenceResult SaveFinalResult()
        {
            return _persistenceService.SaveFinal(State);
        }

        public void CloseApplication()
        {
            Close();
        }

        private BasePage CreateViewForStep(ApplicationStep step)
        {
            return step switch
            {
                ApplicationStep.Welcome => new WelcomePage(this),
                ApplicationStep.MusicianScreening => new MainQuestionPage(this),
                ApplicationStep.Session => new SessionPage(this, Session.Test_1_1),
                ApplicationStep.MusicianQuestionnaire => new QuestionnairePage(this, true),
                ApplicationStep.NonMusicianQuestionnaire => new QuestionnairePage(this, false),
                _ => new ThankYouPage(this)
            };
        }
    }
}
