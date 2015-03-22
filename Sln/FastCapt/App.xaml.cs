using System;
using System.Windows;
using System.Windows.Interop;

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

            ComponentDispatcher.ThreadIdle += OnThreadIdle;
        }

        private void OnThreadIdle(object sender, EventArgs e)
        {
            
        }
    }
}
