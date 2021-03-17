using CalendarHabitsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalendarHabitsApp.UserControls
{
    /// <summary>
    /// Interaction logic for CustomCalendar.xaml
    /// </summary>
    public partial class CustomCalendar : UserControl
    {
        public CustomCalendar()
        {
            InitializeComponent();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MonthDay mDay = ((MonthDay)((Grid)sender).DataContext);
            mDay.Checked = !mDay.Checked;

            MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);

            if (mDay.Checked)
            {
                var habitDay = mainWindow.viewModel.Settings.HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == mDay.Date.ToString("MM/dd/yyyy"));
                if (habitDay == -1)
                {
                    mainWindow.viewModel.Settings.HabitDays.Add(mDay.Date);
                }
            }
            else
            {
                var habitDay = mainWindow.viewModel.Settings.HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == mDay.Date.ToString("MM/dd/yyyy"));
                if (habitDay != -1)
                {
                    mainWindow.viewModel.Settings.HabitDays.RemoveAt(habitDay);
                }
            }
        }
    }
}
