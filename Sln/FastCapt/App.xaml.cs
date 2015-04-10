using System.Windows;
using Squirrel;

namespace FastCapt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UpdateApplication();
        }

        private async void UpdateApplication()
        {
            var updateManager = Container.Current.GetExportedValue<IUpdateManager>();
            await updateManager.UpdateApp();
        }
    }
}
