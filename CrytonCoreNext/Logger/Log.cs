using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using System;
using System.Windows.Media;

namespace CrytonCoreNext.Logger
{
    public class Log : NotificationBase
    {
        private static readonly int Seconds = 4;

        public event EventHandler OnLoggerChanged;

        public Brush Brush { get; set; }

        public ELogLevel LogLevel { get; private set; }

        public string Message { get; set; }

        public Log()
        {
            Message = "";
            Reset();
        }

        public Log(ELogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }

        public void Post(ELogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;

            switch (LogLevel)
            {
                case ELogLevel.Skip:
                    Brush = Brushes.Transparent;
                    break;
                case ELogLevel.Information:
                    Brush = Brushes.LightSteelBlue;
                    break;
                case ELogLevel.Warning:
                    Brush = Brushes.Yellow;
                    break;
                case ELogLevel.Error:
                    Brush = Brushes.OrangeRed;
                    break;
                case ELogLevel.Fatal:
                    Brush = Brushes.Red;
                    break;
                default:
                    Brush = Brushes.CadetBlue;
                    break;
            }

            ActionTimer.InitializeTimerWithAction(ClearLogger, Seconds);
            NotifyLoggerChanged();
        }

        private void Reset(object o = null, EventArgs e = null)
        {
            LogLevel = ELogLevel.Skip;
            Message = string.Empty;
        }

        private void ClearLogger(object sender, EventArgs e)
        {
            Reset();
            NotifyLoggerChanged();
        }

        private void NotifyLoggerChanged()
        {
            OnLoggerChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
