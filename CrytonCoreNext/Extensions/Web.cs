using CrytonCoreNext.Interfaces;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CrytonCoreNext.Services
{
    public class Web : IService
    {
        public JsonWeb WebInfo { get; set; }
        public bool Status { get; set; }

        public bool GetStatus()
        {
            return Status;
        }

        public async Task InitializeService(object obj)
        {
            if (!Equals(obj.GetType(), typeof(InternetConnection)))
                return;

            var internetStatus = obj.GetType().GetProperty("Status").GetValue(obj, null);
            if((bool)internetStatus)
                await Task.Run(() => GetIPAddressPublic()).ConfigureAwait(false);
        }

        public JsonWeb GetAllWebInfo() => WebInfo;
        

        public async Task<(double latitude, double longnitude)> GetGlobalCoordinates()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var info = new WebClient();
                    string respond = "";
                    info.DownloadStringAsync(new Uri("http://ipinfo.io/" + WebInfo?.Ip), respond);

                    var gelocString = WebInfo?.Loc.Split(',');
                    if (gelocString is null)
                        throw new Exception("Data was null");
                    var resOne = double.Parse(gelocString[0], CultureInfo.InvariantCulture);
                    var resTwo = double.Parse(gelocString[1], CultureInfo.InvariantCulture);
                    return (latitude: resOne, longnitude: resTwo);
                }
                catch (Exception)
                {
                    Status = false;
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
                    WebInfo = JsonConvert.DeserializeObject<JsonWeb>(info);
                    Status = true;
                }
                catch (Exception)
                {
                    Status = false;
                }
            });
        }

        public string GetCurrentCity()
        {
            return WebInfo?.City;
        }
        public string GetCurrentCountry()
        {
            return WebInfo?.Country;
        }
        public string GetCurrentRegion()
        {
            return WebInfo?.Region;
        }
        public string GetOrganization()
        {
            return WebInfo?.Org;
        }
        public string GetPostalCode()
        {
            return WebInfo?.Postal;
        }
        public string GetHostname()
        {
            return WebInfo?.Hostname;
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
