using CrytonCoreNext.Interfaces;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media;
using System.Collections.Generic;
using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private const int SecondsInterval = 1;
        private const int MinuteInterval = 1;

        private readonly ITimeDate _timeDate;
        private readonly IInternetConnection _internetConnection;

        public string CurrentTime { get; private set; }
        public string CurrentDay { get; private set; }
        public SolidColorBrush FillDiode { get; private set; }

        public HomeViewModel(ITimeDate timeDate,
            IInternetConnection internetConnection)
        {
            //_timeDate = timeDate;
            //_internetConnection = internetConnection;

            //RefreshTime();
            //RefreshDay();
            //InitializeTimers();
        }

        private void InitializeTimers()
        {
            _ = RunTimer(TimeSpan.FromSeconds(SecondsInterval), 
                new List<Action>() 
                { 
                    () => RefreshTime(),
                    () => RefreshInternetConnection(),
                });

            _ = RunTimer(TimeSpan.FromMinutes(MinuteInterval), 
                new List<Action>() 
                { 
                    () => RefreshDay() 
                });
        }

        private void RefreshInternetConnection()
        {
            FillDiode = _internetConnection.GetColorInternetStatus();
            OnPropertyChanged(nameof(FillDiode));
        }

        private void RefreshTime()
        {
            CurrentTime = _timeDate.GetCurrentTime();
            OnPropertyChanged(nameof(CurrentTime));
        }

        private void RefreshDay()
        {
            CurrentDay = _timeDate.GetCurrentDay();
            OnPropertyChanged(nameof(CurrentDay));
        }

        private static async Task RunTimer(TimeSpan timeSpan, List<Action> actions)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
    }
}
