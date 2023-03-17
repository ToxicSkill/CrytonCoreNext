using CrytonCoreNext.Interfaces.Extras;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrytonCoreNext.InformationsServices
{
    public class InternetConnection : IInternetConnection
    {
        private const string PingStringFirst = "http://www.google.com";
        private const string PingStringSecond = "google.com";
        private const int SecondsDelay = 5;
        private const int InformationBuffer = 32;
        private const int TimeoutLimit = 1000;

        private readonly static Color RedColor = Color.FromRgb(255, 0, 0);
        private readonly static Color GreenColor = Color.FromRgb(0, 255, 0);

        private SolidColorBrush _internetSatusColor = new () { Color = RedColor };
        private bool _status;

        public InternetConnection()
        {
            //InitializeTimer();
        }

        public bool GetInternetStatus() => _status;

        public SolidColorBrush GetColorInternetStatus() => _internetSatusColor;

        private void InitializeTimer()
        {
            _ = RunTimer(TimeSpan.FromSeconds(SecondsDelay), () => UpdateInternetStatus());
        }

        private void UpdateInternetStatus()
        {
            _status = CheckForInternetConnection() || CheckForInternetConnectionSecond();
            _internetSatusColor.Color = _status ? GreenColor : RedColor;
        }

        private static async Task RunTimer(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                action();
            }
        }

        private bool CheckForInternetConnection()
        {
            try
            {
                using WebClient client = new();
                using System.IO.Stream stream = client.OpenRead(PingStringFirst);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckForInternetConnectionSecond()
        {
            try
            {
                Ping myPing = new();
                string host = PingStringSecond;
                byte[] buffer = new byte[InformationBuffer];
                int timeout = TimeoutLimit;
                PingOptions pingOptions = new();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
