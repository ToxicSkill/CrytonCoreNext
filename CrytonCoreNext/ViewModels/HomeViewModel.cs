using CrytonCoreNext.Commands;
using System.Windows.Input;
using CrytonCoreNext.Interfaces;
using System;
using System.Threading.Tasks;
using CrytonCoreNext.Services;
using System.Threading;

namespace CrytonCoreNext.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private const int SecondsInterval = 1;
        private const int MinuteInterval = 1;

        private readonly ITimeDate _timeDate;

        public ICommand NavigateLoginCommand { get; }

        public string CurrentTime { get; private set; }
        public string CurrentDay { get; private set; }

        public HomeViewModel(INavigationService loginNavigationService, ITimeDate timeDate)
        {
            _timeDate = timeDate;

            RefreshTime();
            RefreshDay();
            InitializeTimers();

            NavigateLoginCommand = new NavigateCommand(loginNavigationService);
        }

        private void InitializeTimers()
        {
            _ = RunTimer(TimeSpan.FromSeconds(SecondsInterval), () => RefreshTime());
            _ = RunTimer(TimeSpan.FromMinutes(MinuteInterval), () => RefreshDay());
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

        private static async Task RunTimer(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                action();
            }
        }
    }
}
