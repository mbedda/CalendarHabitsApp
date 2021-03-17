using CalendarHabitsApp.Helpers;
using CalendarHabitsApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using IOPath = System.IO.Path;

namespace CalendarHabitsApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        private ObservableCollection<MonthDay> _selectedMonthDays;
        public ObservableCollection<MonthDay> SelectedMonthDays
        {
            get { return _selectedMonthDays; }
            set { SetProperty(ref _selectedMonthDays, value); }
        }

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set { SetProperty(ref _currentDate, value); }
        }

        public CalendarCell CurrentDateCell { get; set; }

        DispatcherTimer RefreshTimer = new DispatcherTimer();

        public DelegateCommand UpdateCommand { get; set; }

        public MainViewModel()
        {
            UpdateCommand = new DelegateCommand(Update);
            Init();
        }

        private void Update()
        {
            UpdateWallpaper();
        }

        public void Init()
        {
            RefreshTimer.Interval = TimeSpan.FromSeconds(5);
            RefreshTimer.Tick += RefreshTimer_Tick;

            Settings = Common.LoadJson<Settings>(IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "hbts.calhab")); ;

            SelectedMonthDays = new ObservableCollection<MonthDay>();

            CurrentDate = DateTime.Now;
            CurrentDateCell = new CalendarCell();
            FillSelectedMonthInfo();


            UpdateWallpaper();
            RefreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentDate.Day != DateTime.Now.Day)
                UpdateWallpaper();
        }

        public void FillSelectedMonthInfo()
        {
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

                    var habitDay = Settings.HabitDays.ToList().FindIndex(s => s.ToString("MM/dd/yyyy") == indexedDay.ToString("MM/dd/yyyy"));
                    if (habitDay != -1)
                    {
                        habitCheck = true;
                    }

                    if (indexedDay.Month != CurrentDate.Month)
                    {
                        SelectedMonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = false, Checked = habitCheck });
                    }
                    else
                    {
                        SelectedMonthDays.Add(new MonthDay() { Date = indexedDay, FromCurrentMonth = true, Checked = habitCheck });
                    }

                    indexedDay = indexedDay.AddDays(1);
                }
            }
        }

        public void UpdateWallpaper()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (SelectedMonthDays[(i * 7) + j].Date.ToString("MM/dd/yyyy") == CurrentDate.ToString("MM/dd/yyyy"))
                    {
                        CurrentDateCell.X = j + 1;
                        CurrentDateCell.Y = i + 1;
                    }
                }
            }

            Wallpaper.CreateCalendar(Settings.DarkMode, CurrentDate, CurrentDateCell, SelectedMonthDays.ToList(), Settings.HabitDays.ToList());

            CurrentDate = DateTime.Now;
        }

        public void Save()
        {
            Common.SaveJson(Settings, IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "hbts.calhab"));
        }
    }
}
