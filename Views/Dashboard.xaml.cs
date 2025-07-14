using System.Windows;
using Ventuz.ViewModels;

namespace Ventuz.Views
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            this.DataContext = new DashboardViewModel();
        }
    }
}