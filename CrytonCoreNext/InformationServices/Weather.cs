using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.InformationsServices
{
    public class Weather : IService
    {
        public WeatherInfo WholeForecast { get; set; }
        public SingleWeather ActualWeather { get; set; }
        public SunInfo TodaySunInfo { get; set; }
        private string ActualWeatherIcon;
        public bool Status { get; set; }
        private Web _web;

        public bool GetStatus()
        {
            return Status;
        }

        public async Task InitializeService(object obj)
        {
            var type = obj.GetType();
            if (type == typeof(Web))
                _web = (Web)obj;
            await UpdateWeather();
        }

        private class WeatherWebClient : WebClient
        {
            public int Timeout { get; set; }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest lWebRequest = base.GetWebRequest(uri);
                lWebRequest.Timeout = Timeout;
                ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
                return lWebRequest;
            }
        }

        private static string GetRequest(string aURL)
        {
            using var lWebClient = new WeatherWebClient();
            lWebClient.Timeout = 2 * 60 * 1000;
            return lWebClient.DownloadString(aURL);
        }

        private async Task DownloadWeatherForecast((double lat, double lon) geoLocation)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (geoLocation.lat == -1 || geoLocation.lon == -1)
                        throw new Exception("Invalid global coordinates");
                    var weatherUrlStart = "http://www.7timer.info/bin/api.pl?";
                    StringBuilder stringBuilder = new(weatherUrlStart);
                    _ = stringBuilder.Append("lon=" + Math.Round(geoLocation.lon, 2) + "&lat=" + Math.Round(geoLocation.lat, 2));
                    _ = stringBuilder.Append("&product=civil&output=json");
                    var info = GetRequest(stringBuilder.ToString());//.DownloadString(stringBuilder.ToString());
                                                                    // var res = info.GetWebRequest(new Uri(stringBuilder.ToString()));
                    SetSunraiseSunset(geoLocation);
                    SetWholeForecast(JsonConvert.DeserializeObject<WeatherInfo>(info));
                    SetCurrentWeather(FindCurrnetWeather());
                    SetCurrentWeatherIcon();
                }
                catch (Exception)
                {
                    Status = false;
                }
            });
        }

        private void SetSunraiseSunset((double lat, double lon) geoLocation)
        {
            var sunsetSunriseStart = "https://api.sunrise-sunset.org/json?";
            StringBuilder stringBuilder = new(sunsetSunriseStart);
            _ = stringBuilder.Append("lat=" + geoLocation.lat + "&lng=" + geoLocation.lon);
            _ = stringBuilder.Append("&date=today");
            string info = new WebClient().DownloadString(stringBuilder.ToString());
            SetTodaySunInfo(JsonConvert.DeserializeObject<SunInfo>(info));
            //var y = (2 * Math.PI / 365) * (DateTime.Now.DayOfYear - 1 + (DateTime.Now.Hour - 12) / 24);
            //var eqtime = 229.18 * (0.000075 + 0.001868 * Math.Cos(y) - 0.032077 * Math.Sin(y) - .014615 * Math.Cos(2 * y) - 0.040849 * Math.Sin(2 * y));
            //var declin = 0.06918 - 0.399912 * Math.Cos(y) + 0.070257 * Math.Sin(y) - 0.006758 * Math.Cos(2 * y) + 0.000907 * Math.Sin(2 * y) - 0.002687 * Math.Cos(3 * y) + 0.00148 * Math.Sin(3 * y);

            //var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            //var timeOffset = eqtime - 4 * geoLocation.lon + 60 * utcOffset.Hours;
            //var tst = DateTime.Now.Hour * 60 + DateTime.Now.Minute + timeOffset;
            //var ha1 = tst / 4 - 180;
            //var cosPhi = Math.Sin(geoLocation.lat) * Math.Sin(declin) + Math.Cos(geoLocation.lat) * Math.Cos(declin) * Math.Cos(ha1);
            //var cos180Theta =-(Math.Sin(geoLocation.lat) * Math.Cos(cosPhi) - Math.Sin(declin))/(Math.Cos(geoLocation.lat)  * Math.Sin(cosPhi));
            //var ha = Math.Acos(Math.Cos(90.833 / (Math.Cos(geoLocation.lat) * Math.Cos(declin))) - Math.Tan(geoLocation.lat) * Math.Tan(declin));
            //var sunrise = (720 + 4 * (geoLocation.lat - ha) - eqtime) / 60;
            //var snoon = (720 + 4 * geoLocation.lat - eqtime) / 60;

        }

        private void SetTodaySunInfo(SunInfo sunInfo)
        {
            if (sunInfo is not null)
            {
                TodaySunInfo = sunInfo;
                Status = true;
            }
            else
                Status = false;
        }

        private void SetCurrentWeatherIcon()
        {
            var cloundIndex = int.Parse(ActualWeather.Cloudcover);
            var liftedIndex = int.Parse(ActualWeather.LiftedIndex);
            bool night = false;
            var (currentTimeHour, currentTimeMinutes)  = (int.Parse(DateTime.Now.ToString("HH")), int.Parse(DateTime.Now.ToString("mm")));
            string sunset = String.Empty
                ,sunrise = String.Empty;
            try
            {
                sunset = GetCurrentSunset().PadLeft(5, '0');
                sunrise = GetCurrentSunrise().PadLeft(5, '0');
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Status = false;
            }

            var (currentSunsetHours, currentSunsetMinutes) = (int.Parse(sunset[..2]), int.Parse(sunset.Substring(3, 2)));
            var (currentSunriseHours, currentSunriseMinutes) = (int.Parse(sunrise[..2]), int.Parse(sunrise.Substring(3, 2)));

            if (currentTimeHour == currentSunsetHours)
                if (currentTimeMinutes >= currentSunsetMinutes)
                    night = true;
            if (currentTimeHour > currentSunsetHours && currentTimeHour > currentSunriseHours)
                night = true;

            switch (ActualWeather.PrecType.ToString())
            {
                case "rain":
                    if (liftedIndex < -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/HeavyStorm.png";
                    else
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Rain.png";
                    return;
                case "snow":
                    ActualWeatherIcon = "/CrytonCore;component/Assets/Snow.png";
                    return;
                case "none":
                    if (cloundIndex < 3 && liftedIndex > -5)
                        ActualWeatherIcon = night ? "/CrytonCore;component/Assets/Moon.png" : "/CrytonCore;component/Assets/Sun.png";
                    if (cloundIndex > 2 && cloundIndex < 9 && liftedIndex > -5)
                        ActualWeatherIcon = night ? "/CrytonCore;component/Assets/MoonCloud.png" : "/CrytonCore;component/Assets/PartialSun.png";
                    if (cloundIndex == 9 && liftedIndex > -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Cloud.png";
                    if (liftedIndex < -5)
                        ActualWeatherIcon = "/CrytonCore;component/Assets/Storm.png";
                    break;
                default:
                    break;
            }
        }

        public async Task UpdateWeather() => await DownloadWeatherForecast(await _web.GetGlobalCoordinates());

        public SingleWeather GetActualWeather() => ActualWeather;

        public WeatherInfo GetWholeForecast() => WholeForecast;

        public string GetActualWeatherIcon() => ActualWeatherIcon;

        public string GetActualTemperature() => ActualWeather?.Temp.ToString() + "ºC";
        
        public string GetActualWind() => ActualWeather?.Wind10m.Direction.ToString();

        public string GetActualHumidity() => ActualWeather?.Rh2m.ToString();

        public string GetCurrentSunrise()
        {
            if (Status == false || TodaySunInfo == null)
                return string.Empty;
            var sunrise = TodaySunInfo.Details.Sunrise;
            var date = sunrise.ToString().Substring(0, sunrise.Length - 3);
            var time = date.Split(':');
            var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            var sunHour = int.Parse(time[0], System.Globalization.NumberStyles.Integer) + utcOffset.Hours;
            if (sunrise.EndsWith("PM"))
                sunHour += 12;
            var sunriseFinal = string.Concat(sunHour.ToString(), ":", time[1]);
            return sunriseFinal;
        }

        public string GetCurrentSunset()
        {
            if (Status == false || TodaySunInfo == null)
                return string.Empty;
            var sunset = TodaySunInfo.Details.Sunset;
            var date = sunset.ToString().Substring(0, sunset.Length - 3);
            var time = date.Split(':');
            var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            var sunHour = int.Parse(time[0], System.Globalization.NumberStyles.Integer) + utcOffset.Hours;
            if (sunset.EndsWith("PM"))
                sunHour += 12;
            var sunsetFinal = string.Concat(sunHour.ToString(), ":", time[1]);
            return sunsetFinal;
        }

        private SingleWeather FindCurrnetWeather()
        {
            if (WholeForecast is null)
                return new SingleWeather();
            var initDate = WholeForecast.Init;
            DateTime firstDate = DateTime.Now;
            DateTime secondDate = new(
                year: int.Parse(initDate.Substring(0, 4), System.Globalization.NumberStyles.Integer),
                month: int.Parse(initDate.Substring(4, 2), System.Globalization.NumberStyles.Integer),
                day: int.Parse(initDate.Substring(6, 2), System.Globalization.NumberStyles.Integer),
                hour: int.Parse(initDate.Substring(8, 2), System.Globalization.NumberStyles.Integer),
                minute: 00,
                second: 00);

            TimeSpan diff = secondDate.Subtract(firstDate);
            var diffHours = diff.TotalHours;
            var index = Math.Floor(Math.Abs(diffHours) / 3);
            return WholeForecast.Dataseries.Single(x => x.Timepoint == (index * 3).ToString());
        }

        private void SetWholeForecast(WeatherInfo weatherInfo)
        {
            if (weatherInfo is not null)
            {
                WholeForecast = weatherInfo;
                Status = true;
            }
            else
                Status = false;
        }
        private void SetCurrentWeather(SingleWeather singleWeather)
        {
            ActualWeather = singleWeather;
        }
        public class SunInfo
        {
            [JsonProperty("results")]
            public SunDetails Details { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class SunDetails
        {
            [JsonProperty("sunrise")]
            public string Sunrise { get; set; }

            [JsonProperty("sunset")]
            public string Sunset { get; set; }

            [JsonProperty("solar_noon")]
            public string SolarNoon { get; set; }

            [JsonProperty("day_length")]
            public string DayLength { get; set; }

            [JsonProperty("civil_twilight_begin")]
            public string CivilTwilightBegin { get; set; }

            [JsonProperty("civil_twilight_end")]
            public string CivilTwilightEnd { get; set; }

            [JsonProperty("nautical_twilight_begin")]
            public string NauticalTwilightBegin { get; set; }

            [JsonProperty("nautical_twilight_end")]
            public string NauticalTwilightEnd { get; set; }

            [JsonProperty("astronomical_twilight_begin")]
            public string AstronomicalTwilightBegin { get; set; }

            [JsonProperty("astronomical_twilight_end")]
            public string AstronomicalTwilightEnd { get; set; }
        }


        public class WeatherInfo
        {

            [JsonProperty("product")]
            public string Product { get; set; }

            [JsonProperty("init")]
            public string Init { get; set; }

            [JsonProperty("dataseries")]
            public IEnumerable<SingleWeather> Dataseries { get; set; }
        }

        public class SingleWeather
        {

            [JsonProperty("timepoint")]
            public string Timepoint { get; set; }

            [JsonProperty("cloudcover")]
            public string Cloudcover { get; set; }

            [JsonProperty("seeing")]
            public string Seeing { get; set; }

            [JsonProperty("transparency")]
            public string Transparency { get; set; }

            [JsonProperty("lifted_index")]
            public string LiftedIndex { get; set; }

            [JsonProperty("rh2m")]
            public string Rh2m { get; set; }

            [JsonProperty("wind10m")]
            public Wind Wind10m { get; set; }

            [JsonProperty("temp2m")]
            public string Temp { get; set; }

            [JsonProperty("prec_type")]
            public string PrecType { get; set; }
        }

        public class Wind
        {
            [JsonProperty("direction")]
            public string Direction { get; set; }

            [JsonProperty("speed")]
            public string Speed { get; set; }
        }
    }
}
