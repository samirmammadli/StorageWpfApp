using StorageWpfApp.View;
using System.Windows;


namespace StorageWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
        public App()
        {
            var app = new AppView();
            using (var locator = new ViewModelLocator())
            {
                app.DataContext = locator.appViewModel;
                app.ShowDialog();
            }
        }
    }
}
