using CrytonCoreNext.Interfaces;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace CrytonCoreNext.InformationsServices
{
    public class TimeDate : ITimeDate
    {
        private const double SecondsDelay = 1;
        private const double MinutesDelay = 1;

        private string _currentTime;
        private string _currentDay;

        public TimeDate()
        {
            //SetCurrentTime();
            //SetCurrentDay();
            //InitializeTimers();
        }

        private void InitializeTimers()
        {
            _ = RunTimer(TimeSpan.FromSeconds(SecondsDelay), () => SetCurrentTime());
            _ = RunTimer(TimeSpan.FromMinutes(MinutesDelay), () => SetCurrentDay());
        }

        public string GetCurrentDay() => _currentDay;

        public string GetCurrentTime() => _currentTime;

        private static async Task RunTimer(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                action();
            }
        }

        private void SetCurrentDay()
        {
            CultureInfo myCI = new("en-EN");
            string newDay = myCI.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            _currentDay = char.ToUpper(newDay[0]) + newDay.Substring(1);
        }

        private void SetCurrentTime()
        {
            _currentTime = DateTime.Now.ToString("HH:mm");
        }
    }
}
