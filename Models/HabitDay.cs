using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarHabitsApp.Models
{
    public class HabitDay : BindableBase
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set { SetProperty(ref _checked, value); }
        }
    }
}
