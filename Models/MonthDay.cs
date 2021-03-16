using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarHabitsApp.Models
{
    public class MonthDay
    {
        public DateTime Date { get; set; }

        public bool FromCurrentMonth { get; set; }

        public bool Checked { get; set; }
    }
}
