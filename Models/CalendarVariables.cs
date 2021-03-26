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
            canvasStartX = 459;
            canvasStartY = 151;

            calendarCellW = 142;
            calendarCellH = 142;

            calendarCellBorder = 1;

            crossoutImageW = 88;
            crossoutImageH = 84;

            currentDayHighlightImageW = 112;
            currentDayHighlightImageH = 112;
        }
    }
}
