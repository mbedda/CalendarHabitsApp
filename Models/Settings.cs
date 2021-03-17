using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<DateTime> _habitDays;
        public ObservableCollection<DateTime> HabitDays
        {
            get { return _habitDays; }
            set { SetProperty(ref _habitDays, value); }
        }

        public Settings()
        {
            HabitDays = new ObservableCollection<DateTime>();
        }
    }
}
