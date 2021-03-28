using CalendarHabitsApp.ViewModels;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CalendarHabitsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainViewModel();

            viewModel.InitAsync().ContinueWith((t1) =>
            {
                if (viewModel.Settings.StartMinimized)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        WindowState = WindowState.Minimized;
                    });
                }
            });

            InitializeComponent();

            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.Save();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    ShowInTaskbar = true;
                    break;
                case WindowState.Minimized:
                    Hide();
                    ShowInTaskbar = false;
                    break;
                case WindowState.Normal:
                    ShowInTaskbar = true;
                    break;
            }
        }

        private void chkStartUp_Checked(object sender, RoutedEventArgs e)
        {
            InstallMeOnStartUp();
        }

        void InstallMeOnStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();

                //string appPath = curAssembly.Location.Replace(".dll", ".exe");
                var process = Process.GetCurrentProcess(); // Or whatever method you are using
                string appPath = process.MainModule.FileName;

                //if (viewModel.Settings.StartMinimized)
                //appPath += " --start-minimized";

                if (chkStartUp.IsChecked.Value)
                    key.SetValue(curAssembly.GetName().Name, appPath);
                else
                    key.DeleteValue(curAssembly.GetName().Name, false);
            }
            catch { }
        }
    }
}
