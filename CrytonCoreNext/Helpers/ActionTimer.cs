using System;
using System.Windows.Threading;

namespace CrytonCoreNext.Helpers
{
    public static class ActionTimer
    {
        private static DispatcherTimer _timer = new();

        public static void InitializeTimerWithAction(Action<object, EventArgs> obj, int seconds)
        {
            _timer.Tick += new EventHandler(obj);
            _timer.Interval = new TimeSpan(0, 0, seconds);
            _timer.Start();
        }

        public static void StopTimer()
        {
            _timer.Stop();
        }
    }
}
