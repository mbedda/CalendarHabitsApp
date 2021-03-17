using CalendarHabitsApp.Helpers;
using CalendarHabitsApp.Models;
using CalendarHabitsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    }
}
