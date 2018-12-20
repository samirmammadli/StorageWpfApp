using StorageWpfApp.View;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
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
            var culture = new CultureInfo("en-US") { DateTimeFormat = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat };

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            var app = new AppView();
            using (var locator = new ViewModelLocator())
            {
                app.DataContext = locator.appViewModel;
                app.Closing += Window_Closing;
                locator.appViewModel.CloseAction = () => app.Close();
                app.ShowDialog();
            }
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Выйти из программы?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                e.Cancel = true;
            return;
        }
    }
}
