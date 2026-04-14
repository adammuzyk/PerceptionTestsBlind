using System.Windows.Controls;
using PerceptionTests.Services;

namespace PerceptionTests.Pages
{
    public class BasePage : ContentControl
    {
        protected IApplicationController Controller { get; private set; }

        public BasePage(IApplicationController controller, string title = null)
        {
            Controller = controller;
            if (title != null)
            {
                controller.WindowTitle = title;
            }
        }
    }
}
