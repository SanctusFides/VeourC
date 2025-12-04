using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace Veour.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void GitHub_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/SanctusFides/VeourC") { UseShellExecute = true });
        }

        private void LinkedIn_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            Process.Start(new ProcessStartInfo("https://www.linkedin.com/in/john-hines-788893183/") { UseShellExecute = true });
        }
    }
}
