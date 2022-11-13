using CrytonCoreNext.Enums;
using System;

namespace CrytonCoreNext.Logger
{
    public class Log
    {
        public ELogLevel LogLevel { get; private set; }

        public string Message { get; private set; }


        public Log()
        {
            Message = "";
            Reset();
        }

        public Log(ELogLevel logLevel, string message, int seconds = 4)
        {
            LogLevel = logLevel;
            Message = message;
        }

        public void Set(ELogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }

        public void Reset(object o = null, EventArgs e = null)
        {
            LogLevel = ELogLevel.Skip;
            Message = string.Empty;
        }
    }
}
