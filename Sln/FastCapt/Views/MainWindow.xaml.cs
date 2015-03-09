using System.Windows;
using System.Windows.Input;
using FastCapt.Controls;
using FastCapt.ViewModels;

namespace FastCapt.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="MainWindow"/> class.
        /// </summary>
        static MainWindow()
        {
            InitializeCommands();
        }

        /// <summary>
        /// Initializes instance members of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                DataContext = new MainViewModel();
            };
        }

        #endregion

        #region "Methods"

        private static void InitializeCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(MainWindow),
                new CommandBinding(WindowCommands.CloseCommand,
                    (sender, args) =>
                    {
                        var wnd = (MainWindow)sender;
                        wnd.Close();
                    }));

            CommandManager.RegisterClassCommandBinding(typeof(MainWindow),
                new CommandBinding(WindowCommands.MinimizeCommand,
                    (sender, args) =>
                    {
                        var wnd = (MainWindow)sender;
                        wnd.WindowState = WindowState.Minimized;
                    }));
        } 

        #endregion
    }
}
