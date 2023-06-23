using CrytonCoreNext.Interfaces.Extras;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CrytonCoreNext.InformationsServices
{
    public class Web : IWeb
    {
        public JsonWeb? _webInfo;
        private bool _status;


        public bool GetWebStatus() => _status;
        

        public async Task InitializeService(object obj)
        {
            if (!Equals(obj.GetType(), typeof(InternetConnection)))
                return;

            var internetStatus = obj.GetType().GetProperty("Status").GetValue(obj, null);
            if((bool)internetStatus)
                await Task.Run(() => GetIPAddressPublic()).ConfigureAwait(false);
        }

        public JsonWeb GetAllWebInfo() => _webInfo;
        

        public async Task<(double latitude, double longnitude)> GetGlobalCoordinates()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var info = new WebClient();
                    string respond = "";
                    info.DownloadStringAsync(new Uri("http://ipinfo.io/" + _webInfo?.Ip), respond);

                    var gelocString = _webInfo?.Loc.Split(',');
                    if (gelocString is null)
                        throw new Exception("Data was null");
                    var resOne = double.Parse(gelocString[0], CultureInfo.InvariantCulture);
                    var resTwo = double.Parse(gelocString[1], CultureInfo.InvariantCulture);
                    return (latitude: resOne, longnitude: resTwo);
                }
                catch (Exception)
                {
                    _status = false;
                    return (latitude: -1, longnitude: -1);
                }
            });
        }

        private async Task GetIPAddressPublic()
        {
            await Task.Run(() =>
            {
                try
                {
                    string address = "";
                    WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                    using (WebResponse response = request.GetResponse())
                    using (StreamReader stream = new(response.GetResponseStream()))
                        address = stream.ReadToEnd();
                    int first = address.IndexOf("Address: ") + 9;
                    int last = address.LastIndexOf("</body>");
                    address = address.Substring(first, last - first);
                    string info = new WebClient().DownloadString("http://ipinfo.io/" + address);
                    _webInfo = JsonConvert.DeserializeObject<JsonWeb>(info);
                    _status = true;
                }
                catch (Exception)
                {
                    _status = false;
                }
            });
        }

        public string GetCurrentCity()
        {
            return _webInfo?.City;
        }
        public string GetCurrentCountry()
        {
            return _webInfo?.Country;
        }
        public string GetCurrentRegion()
        {
            return _webInfo?.Region;
        }
        public string GetOrganization()
        {
            return _webInfo?.Org;
        }
        public string GetPostalCode()
        {
            return _webInfo?.Postal;
        }
        public string GetHostname()
        {
            return _webInfo?.Hostname;
        }

        public class JsonWeb
        {

            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("hostname")]
            public string Hostname { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("loc")]
            public string Loc { get; set; }

            [JsonProperty("org")]
            public string Org { get; set; }

            [JsonProperty("postal")]
            public string Postal { get; set; }
        }
    }
}
