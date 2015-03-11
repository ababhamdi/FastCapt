using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using FastCapt.Services.Interfaces;
using FastCapt.Views;

namespace FastCapt.ApplicationModel
{
    [Export(typeof(IApplication))]
    internal class FastCaptApplication : Application, IApplication
    {
        #region "Fields"

        private readonly IEnumerable<IStartupService> _startupServices;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="FastCaptApplication"/> class.
        /// </summary>
        [ImportingConstructor]
        public FastCaptApplication([ImportMany]IEnumerable<IStartupService> startupServices)
        {
            _startupServices = startupServices;

            var source = new Uri("/Resources/Styles.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(new ResourceDictionary{Source = source});
        }

        #endregion

        #region "Methods"

        protected override void OnStartup(StartupEventArgs e)
        {
            RunStartupServices();
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StopStartupServices();
            base.OnExit(e);
        }

        private void RunStartupServices()
        {
            foreach (var service in _startupServices)
            {
                service.Run();
            }
        }

        private void StopStartupServices()
        {
            foreach (var service in _startupServices)
            {
                service.Stop();
            }
        }

        public void Start()
        {
            this.Run();
        }

        #endregion
    }
}