using System.Windows.Input;

namespace FastCapt.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindowCommands
    {
        private static RoutedUICommand _closeCommand;
        private static RoutedUICommand _minimizeCommand;

        /// <summary>
        /// Close the window.
        /// </summary>
        public static RoutedUICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RoutedUICommand("Close",
                    "Close",
                    typeof(WindowCommands)));
            }
        }

        /// <summary>
        /// Minimize the Window to the taskbar.
        /// </summary>
        public static RoutedUICommand MinimizeCommand
        {
            get
            {
                return _minimizeCommand ?? (_minimizeCommand = new RoutedUICommand("Minimize",
                    "Minimize",
                    typeof(WindowCommands)));
            }
        }
    }
}
