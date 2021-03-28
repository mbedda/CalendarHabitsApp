using System.Windows;
using System.Windows.Input;

namespace CalendarHabitsApp
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DCommand
                {
                    CommandAction = () => ShowWindow(),
                    CanExecuteFunc = () => true
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DCommand
                {
                    CommandAction = () => HideWindow(),
                    CanExecuteFunc = () => Application.Current.MainWindow.WindowState != WindowState.Minimized
                };
            }
        }

        public void HideWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            Application.Current.MainWindow.ShowInTaskbar = false;
        }

        public void ShowWindow()
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Topmost = true;  // important
            Application.Current.MainWindow.Topmost = false; // important
            Application.Current.MainWindow.Focus();         // important
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DCommand { CommandAction = () => Application.Current.Shutdown()};
            }
        }
    }
}
