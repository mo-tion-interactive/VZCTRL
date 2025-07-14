using System.Windows;
using Ventuz.Views;

namespace Ventuz
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var dashboard = new Dashboard(); // Dashboard.xaml
            dashboard.Show();
        }

    }
}