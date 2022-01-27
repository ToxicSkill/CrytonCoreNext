using CrytonCoreNext.Commands;
using CrytonCoreNext.Services;
using System.Windows.Input;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CrytonCoreNext.ViewModels;

namespace CrytonCoreNext.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ICommand NavigateLoginCommand { get; }

        public HomeViewModel(INavigationService loginNavigationService)
        {
            NavigateLoginCommand = new NavigateCommand(loginNavigationService);
            _ = Task.Run(() => InitializeServices());
            InitializeTimers();
        }

        private readonly DispatcherTimer _secondsTime = new()
        {
            Interval = TimeSpan.FromSeconds(SecondsDelay)
        };
        private readonly DispatcherTimer _minutesTime = new()
        {
            Interval = TimeSpan.FromMinutes(MinutesDelay)
        };

        private readonly TimeDate _actualTimeDate = new();
        private SolidColorBrush _internetColorDiode = new();
        private readonly List<Tuple<IService, string>> _services = new();

        private string _currentTime;
        private string _currentDay;
        private string _toolTip;
        private string _actualTemperature;
        private string _actualHumidity;
        private string _actualWind;
        private string _actualWeatherIcon;
        private string _actualCity;
        private string _actualRegion;
        private string _actualCountry;
        private string _sunrise;
        private string _sunset;

        private const int SecondsDelay = 1;
        private const int MinutesDelay = 1;

        private async Task InitializeServices()
        {
            _services.Add(new Tuple<IService, string>(new InternetConnection(), nameof(InternetConnection)));
            _services.Add(new Tuple<IService, string>(new Web(), nameof(Web)));
            _services.Add(new Tuple<IService, string>(new Weather(), nameof(Weather)));

            await InitializeServiceByName(nameof(InternetConnection), null);
            await InitializeServiceByName(nameof(Web), GetServiceByName(nameof(InternetConnection)));
            await InitializeServiceByName(nameof(Weather), GetServiceByName(nameof(Web)));

            await UpdateUIServices(nameof(InternetConnection));
            await UpdateUIServices(nameof(Web));
            await UpdateUIServices(nameof(Weather));
        }

        private async Task UpdateUIServices(string serviceName)
        {
            await Thread;
            var service = GetServiceByName(serviceName);
            switch (serviceName)
            {
                case nameof(InternetConnection):
                    InternetConnection internet = service as InternetConnection;
                    FillDiode = internet.GetInternetStatusColor();
                    ToolTip = internet.GetInternetStatusString();
                    break;
                case nameof(Web):
                    Web web = service as Web;
                    WebVisibility = web.Status ? Visibility.Visible : Visibility.Hidden;
                    ActualCity = web.GetCurrentCity();
                    ActualCountry = web.GetCurrentCountry();
                    ActualRegion = web.GetCurrentRegion();
                    break;
                case nameof(Weather):
                    Weather weather = service as Weather;
                    WeatherVisibility = weather.Status ? Visibility.Visible : Visibility.Hidden;
                    ActualTemperature = weather.GetActualTemperature();
                    ActualHumidity = weather.GetActualHumidity();
                    ActualWind = weather.GetActualWind();
                    ActualWeatherIcon = weather.GetActualWeatherIcon();
                    Sunrise = weather.GetCurrentSunrise();
                    Sunset = weather.GetCurrentSunset();
                    break;
                default:
                    break;
            }
        }

        private IService GetServiceByName(string name)
        {
            return _services.Where(x => x.Item2 == name).Select(x => x.Item1).FirstOrDefault();
        }

        private async Task InitializeServiceByName(string name, object obj)
        {
            var index = _services.IndexOf(_services.Where(x => x.Item2 == name).FirstOrDefault());
            await _services[index].Item1.InitializeService(obj);
        }

        private void InitializeTimers()
        {
            _secondsTime.Tick += (s, e) => Task.Run(() => SecondTimer_Tick(s, e));
            _secondsTime.Start();
            _minutesTime.Tick += (s, e) => Task.Run(() => MinuteTimer_Tick(s, e));
            _minutesTime.Start();
        }

        public static Transform SubtitleTransform => new ScaleTransform(0.9, 1);

        private async Task SecondTimer_Tick(object sender, EventArgs e)
        {
            await InitializeServiceByName(nameof(InternetConnection), null);
            await UpdateUIServices(nameof(InternetConnection));
            await UpdateTime();
        }

        private async Task UpdateTime()
        {
            CurrentTime = _actualTimeDate.GetCurrentTime();
            CurrentDay = _actualTimeDate.GetCurrentDay();
            await Thread;
        }

        private async Task MinuteTimer_Tick(object sender, EventArgs e)
        {
            await InitializeServiceByName(nameof(Web), GetServiceByName(nameof(InternetConnection)));
            await InitializeServiceByName(nameof(Weather), GetServiceByName(nameof(Web)));
            await UpdateUIServices(nameof(Web));
            await UpdateUIServices(nameof(Weather));
        }

        private Visibility _webVisibility = Visibility.Hidden;
        private Visibility _weatherVisibility = Visibility.Hidden;

        public Visibility WebVisibility
        {
            get => _webVisibility;
            set
            {
                _webVisibility = value;
                OnPropertyChanged(nameof(WebVisibility));
            }
        }

        public Visibility WeatherVisibility
        {
            get => _weatherVisibility;
            set
            {
                _weatherVisibility = value;
                OnPropertyChanged(nameof(WeatherVisibility));
            }
        }

        public string ActualCity
        {
            get => _actualCity;
            set
            {
                _actualCity = value;
                OnPropertyChanged(nameof(ActualCity));
            }
        }
        public string ActualRegion
        {
            get => _actualRegion;
            set
            {
                _actualRegion = value;
                OnPropertyChanged(nameof(ActualRegion));
            }
        }
        public string ActualCountry
        {
            get => _actualCountry;
            set
            {
                _actualCountry = value;
                OnPropertyChanged(nameof(ActualCountry));
            }
        }
        public string ActualTemperature
        {
            get => _actualTemperature;
            set
            {
                _actualTemperature = value;
                OnPropertyChanged(nameof(ActualTemperature));
            }
        }
        public string ActualHumidity
        {
            get => _actualHumidity;
            set
            {
                _actualHumidity = value;
                OnPropertyChanged(nameof(ActualHumidity));
            }
        }
        public string ActualWind
        {
            get => _actualWind;
            set
            {
                _actualWind = value;
                OnPropertyChanged(nameof(ActualWind));
            }
        }
        public string ActualWeatherIcon
        {
            get => _actualWeatherIcon;
            set
            {
                _actualWeatherIcon = value;
                OnPropertyChanged(nameof(ActualWeatherIcon));
            }
        }
        public string Sunrise
        {
            get => _sunrise;
            set
            {
                _sunrise = value;
                OnPropertyChanged(nameof(Sunrise));
            }
        }
        public string Sunset
        {
            get => _sunset;
            set
            {
                _sunset = value;
                OnPropertyChanged(nameof(Sunset));
            }
        }


        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        public string CurrentDay
        {
            get => _currentDay;
            set
            {
                _currentDay = value;
                OnPropertyChanged(nameof(CurrentDay));
            }
        }
        public string ToolTip
        {
            get => _toolTip;
            set
            {
                _toolTip = value;
                OnPropertyChanged(nameof(ToolTip));
            }
        }
        public SolidColorBrush FillDiode
        {
            get => _internetColorDiode;
            set
            {
                _internetColorDiode = value;
                OnPropertyChanged(nameof(FillDiode));
            }
        }

        public static DispatcherAwaiter Thread => new();

        public struct DispatcherAwaiter : INotifyCompletion
        {
            public bool IsCompleted => Application.Current.Dispatcher.CheckAccess();

            public void OnCompleted(Action continuation) => Application.Current.Dispatcher.Invoke(continuation);

            public void GetResult() { }

            public DispatcherAwaiter GetAwaiter()
            {
                return this;
            }
        }
    }
}
