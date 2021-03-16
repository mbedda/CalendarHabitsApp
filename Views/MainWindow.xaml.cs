using CalendarHabitsApp.Helpers;
using CalendarHabitsApp.Models;
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
        int cellx = 0;
        int celly = 0;

        String MonthName = "January";

        DispatcherTimer refreshTimer = new DispatcherTimer();

        DateTime currentDate = new DateTime();

        public ObservableCollection<DateTime> HabitDays { get; set; }
        public ObservableCollection<MonthDay> MonthDays { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            HabitDays = Common.LoadJson<ObservableCollection<DateTime>>(
                System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hbts.calhab"));
            if (HabitDays == null)
            {
                HabitDays = new ObservableCollection<DateTime>();
            }

            MonthDays = new ObservableCollection<MonthDay>();
            MonthDays.CollectionChanged += MonthDays_CollectionChanged;

            currentDate = DateTime.Now;
            FillMonthInfo();

            refreshTimer.Interval = TimeSpan.FromSeconds(5);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            UpdateWallpaper();
        }

        private void MonthDays_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DaysList.ItemsSource = null;
            DaysList.ItemsSource = MonthDays;
            //throw new NotImplementedException();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (currentDate.Day != DateTime.Now.Day)
                UpdateWallpaper();
        }

        public void UpdateWallpaper()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (MonthDays[(i*7)+j].Date.ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                    {
                        cellx = j + 1;
                        celly = i + 1;
                    }
                }
            }

            Wallpaper.Set(null, Wallpaper.Style.Centered, cellx, celly, DateTime.Now.Year, currentDate, MonthDays.ToList(), HabitDays.ToList());

            currentDate = DateTime.Now;
        }

        public void FillMonthInfo()
        {
            MonthName = DateTime.Now.ToString("MMMM");

            DateTime indexedDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int tmpdec = 0;

            while (!indexedDay.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                tmpdec--;
                indexedDay = indexedDay.AddDays(tmpdec);
            }


            for (int i = 0; i < 5; i++)
            {
                List<int> colList = new List<int>();

                for (int j = 0; j < 7; j++)
                {
                    bool habitCheck = false;

                    var habitDay = HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == indexedDay.ToString("MM/dd/yyyy"));
                    if (habitDay != -1)
                    {
                        habitCheck = true;
                    }

                    if (indexedDay.Month != currentDate.Month)
                    {
                        MonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = false, Checked = habitCheck });
                    }
                    else
                    {
                        MonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = true, Checked = habitCheck });
                    }

                    indexedDay = indexedDay.AddDays(1);
                }
            }
        }

        private void DaysList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (MonthDay monthDay in e.AddedItems)
            {
                monthDay.Checked = true;

                var habitDay = HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == monthDay.Date.ToString("MM/dd/yyyy"));
                if (habitDay == -1)
                {
                    HabitDays.Add(monthDay.Date);
                }
            }

            foreach (MonthDay monthDay in e.RemovedItems)
            {
                monthDay.Checked = false;

                var habitDay = HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == monthDay.Date.ToString("MM/dd/yyyy"));
                if (habitDay != -1)
                {
                    HabitDays.RemoveAt(habitDay);
                }
            }

            UpdateWallpaper();
            //DaysList.ItemsSource = null;
            //DaysList.ItemsSource = MonthDays;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Common.SaveJson(HabitDays, 
                System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hbts.calhab"));
        }
    }
}
