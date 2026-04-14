using System.Windows.Input;
using PerceptionTests.Services;

namespace PerceptionTests.Pages
{
    /// <summary>
    /// Interaction logic for ThankYouPage.xaml
    /// </summary>
    public partial class ThankYouPage
    {
        public ThankYouPage(IApplicationController controller) : base(controller)
        {
            InitializeComponent();
            SaveToFile();
        }

        private void SaveToFile()
        {
            var result = Controller.SaveFinalResult();
            if (result.Success)
            {
                MainText.Text =
                    "Thank you for participating.\n\n" +
                    "Results were saved successfully.\n\n" +
                    "Saved file:\n" + result.FilePath;
            }
            else
            {
                MainText.Text =
                    "The experiment is complete, but the final results file could not be saved.\n\n" +
                    result.ErrorMessage +
                    "\n\nPlease do not close the application until the error details have been recorded.";
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Controller.CloseApplication();
        }

        private void OnCloseClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Controller.CloseApplication();
        }
    }
}
