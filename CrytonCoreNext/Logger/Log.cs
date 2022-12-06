using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrytonCoreNext.Logger
{
    public class Log : NotificationBase
    {
        private const int Seconds = 2;

        public event EventHandler OnLoggerChanged;

        public Brush Brush { get; set; } = Brushes.White;

        public ELogLevel LogLevel { get; private set; }

        public string Message { get; set; }

        public Log()
        {
            Message = "";
        }

        public Log(ELogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }

        public async Task Post(ELogLevel logLevel, string message, int seconds = Seconds)
        {
            LogLevel = logLevel;
            Message = message;

            switch (LogLevel)
            {
                case ELogLevel.Skip:
                    Brush = Brushes.Transparent;
                    break;
                case ELogLevel.Information:
                    Brush = Brushes.SkyBlue;
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

            NotifyLoggerChanged();
            await Task.Delay(seconds * 1000);
            ClearLogger();
            NotifyLoggerChanged();
        }

        private void ClearLogger()
        {
            LogLevel = ELogLevel.Skip;
            Message = string.Empty;
        }

        private void NotifyLoggerChanged()
        {
            OnLoggerChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
