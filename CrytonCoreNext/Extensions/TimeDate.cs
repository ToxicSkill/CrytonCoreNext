using System;
using System.Globalization;

namespace CrytonCoreNext.Services
{
    public class TimeDate
    {
        private readonly App app = System.Windows.Application.Current as App;
        public string CurrentTime { get; set; }
        public string CurrentDay { get; set; }

        public TimeDate()
        {
            CurrentTime = GetCurrentTime();
            CurrentDay = GetCurrentDay();
        }

        internal string GetCurrentDay()
        {
            CultureInfo myCI = new("en-EN");
            string newDay = myCI.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            CurrentDay = char.ToUpper(newDay[0]) + newDay.Substring(1);
            return CurrentDay;
        }

        internal string GetCurrentTime()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
            return CurrentTime;
        }
    }
}
