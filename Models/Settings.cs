using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace CalendarHabitsApp.Models
{
    public class Settings : BindableBase
    {
        private bool _darkMode;
        public bool DarkMode
        {
            get { return _darkMode; }
            set { SetProperty(ref _darkMode, value); }
        }

        private bool _runOnStartup;
        public bool RunOnStartup
        {
            get { return _runOnStartup; }
            set { SetProperty(ref _runOnStartup, value); }
        }

        private bool _startMinimized;
        public bool StartMinimized
        {
            get { return _startMinimized; }
            set { SetProperty(ref _startMinimized, value); }
        }

        private bool _pauseWallpaperUpdate;
        public bool PauseWallpaperUpdate
        {
            get { return _pauseWallpaperUpdate; }
            set { SetProperty(ref _pauseWallpaperUpdate, value); }
        }

        private ObservableCollection<DateTime> _habitDays;
        public ObservableCollection<DateTime> HabitDays
        {
            get { return _habitDays; }
            set { SetProperty(ref _habitDays, value); }
        }

        private DateTime _currentDate;
        [IgnoreDataMember]
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set { SetProperty(ref _currentDate, value); }
        }

        private ObservableCollection<MonthDay> _selectedMonthDays;
        [IgnoreDataMember]
        public ObservableCollection<MonthDay> SelectedMonthDays
        {
            get { return _selectedMonthDays; }
            set { SetProperty(ref _selectedMonthDays, value); }
        }

        public Settings()
        {
            HabitDays = new ObservableCollection<DateTime>();
            SelectedMonthDays = new ObservableCollection<MonthDay>();
            CurrentDate = DateTime.Now;
        }
    }
}
