using CrytonCoreNext.Abstract;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Static;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrytonCoreNext.Logger
{
    public class Log : NotificationBase
    {
        private const int Seconds = 2;

        private int _invokeCounter = 0;

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
            _invokeCounter++;
            LogLevel = logLevel;
            Message = message;

            switch (LogLevel)
            {
                case ELogLevel.Skip:
                    Brush = new SolidColorBrush(ColorStatus.Transparent);
                    break;
                case ELogLevel.Information:
                    Brush = new SolidColorBrush(ColorStatus.Blue);
                    break;
                case ELogLevel.Warning:
                    Brush = new SolidColorBrush(ColorStatus.Yellow);
                    break;
                case ELogLevel.Error:
                    Brush = new SolidColorBrush(ColorStatus.Red);
                    break;
                case ELogLevel.Fatal:
                    Brush = new SolidColorBrush(ColorStatus.Red);
                    break;
                default:
                    Brush = new SolidColorBrush(ColorStatus.Blue);
                    break;
            }

            NotifyLoggerChanged();
            await Task.Delay(seconds * 1000);
            ClearLogger();
            NotifyLoggerChanged();
        }

        private void ClearLogger()
        {
            _invokeCounter--;
            if (_invokeCounter == 0)
            {
                LogLevel = ELogLevel.Skip;
                Message = string.Empty;
            }
        }

        private void NotifyLoggerChanged()
        {
            OnLoggerChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
