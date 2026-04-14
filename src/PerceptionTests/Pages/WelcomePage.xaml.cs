using PerceptionTests.Models;
using PerceptionTests.Services;
using System.Windows;

namespace PerceptionTests.Pages
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage
    {
        public WelcomePage(IApplicationController controller)
            : base(controller, ApplicationInfo.DisplayName)
        {
            InitializeComponent();
            MainText.Text = ApplicationInfo.DisplayName + "\nVersion " + ApplicationInfo.Version + "\n\nClick to begin.";
        }

        private void OnBeginClicked(object sender, RoutedEventArgs e)
        {
            Controller.NavigateTo(ApplicationStep.MusicianScreening);
        }
    }
}
