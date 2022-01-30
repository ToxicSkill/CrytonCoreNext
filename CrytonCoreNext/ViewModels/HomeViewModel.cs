using CrytonCoreNext.Commands;
using System.Windows.Input;
using CrytonCoreNext.Interfaces;
using System;
using System.Threading.Tasks;
using CrytonCoreNext.Services;
using System.Threading;
using System.Windows.Media;
using System.Collections.Generic;

namespace CrytonCoreNext.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private const int SecondsInterval = 1;
        private const int MinuteInterval = 1;

        private readonly ITimeDate _timeDate;
        private readonly IInternetConnection _internetConnection;

        public ICommand NavigateLoginCommand { get; }

        public string CurrentTime { get; private set; }
        public string CurrentDay { get; private set; }
        public SolidColorBrush FillDiode { get; private set; }

        public HomeViewModel(INavigationService loginNavigationService, 
            ITimeDate timeDate,
            IInternetConnection internetConnection)
        {
            _timeDate = timeDate;
            _internetConnection = internetConnection;

            RefreshTime();
            RefreshDay();
            InitializeTimers();

            NavigateLoginCommand = new NavigateCommand(loginNavigationService);
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
