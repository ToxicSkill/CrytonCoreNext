using System;
using System.Windows.Threading;

namespace CrytonCoreNext.Helpers
{
    public static class ActionTimer
    {
        private const int DefaultDurationSeconds = 2;

        private static readonly DispatcherTimer _timer = new();

        public static void InitializeTimerWithAction(Action<object, EventArgs> obj, int durationSeconds = DefaultDurationSeconds)
        {
            _timer.Tick += new EventHandler((o, e) => StopTimer(obj));
            _timer.Interval = new TimeSpan(0, 0, durationSeconds > 0 ? durationSeconds : DefaultDurationSeconds);
            _timer.Start();
        }

        private static void StopTimer(Action<object, EventArgs> obj)
        {
            obj.Invoke(null, null);
            _timer.Stop();
        }
    }
}
