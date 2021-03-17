using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarHabitsApp.Models
{
    public class CalendarVariables
    {
        public int canvasStartX { get; set; }
        public int canvasStartY { get; set; }

        public int calendarCellW { get; set; }
        public int calendarCellH { get; set; }

        public int calendarCellBorder { get; set; }

        public int crossoutImageW { get; set; }
        public int crossoutImageH { get; set; }

        public int currentDayHighlightImageW { get; set; }
        public int currentDayHighlightImageH { get; set; }

        public CalendarVariables()
        {
            canvasStartX = 344;
            canvasStartY = 145;

            calendarCellW = 175;
            calendarCellH = 176;

            calendarCellBorder = 1;

            crossoutImageW = 109;
            crossoutImageH = 104;

            currentDayHighlightImageW = 140;
            currentDayHighlightImageH = 140;
        }
    }
}
