using CalendarHabitsApp.Helpers;
using CalendarHabitsApp.Models;
using CalendarHabitsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CalendarHabitsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel;
        public bool calendarInitComplete;

        public MainWindow()
        {
            viewModel = new MainViewModel();

            calendarInitComplete = false;

            InitializeComponent();

            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            calendarInitComplete = true;

            if (viewModel.Settings.StartMinimized)
            {
                ShowInTaskbar = false;
                WindowState = WindowState.Minimized;
            }

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
                    Application.Current.MainWindow.ShowInTaskbar = true;
                    break;
                case WindowState.Minimized:
                    Application.Current.MainWindow.ShowInTaskbar = false;
                    break;
                case WindowState.Normal:
                    Application.Current.MainWindow.ShowInTaskbar = true;
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
                if (chkStartUp.IsChecked.Value)
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location.Replace(".dll", ".exe"));
                else
                    key.DeleteValue(curAssembly.GetName().Name, false);
            }
            catch { }
        }

        private void chkStartUp_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
