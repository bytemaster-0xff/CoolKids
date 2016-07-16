using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Models
{
    public class SmartThingsHub
    {
        public String Id { get; set; }
        public String IPAddress { get; set; }
        public String Port { get; set; }
        public DateTime LastPing { get; set; }

        public async Task<bool> SendAsync(String content)
        {
            try
            {                
                var client = new HttpClient();
                Port = 39500.ToString();
                Debug.WriteLine("--------------------------------------------------------------");
                Debug.WriteLine(String.Format("Sending Notification To: {0} - {1}", IPAddress, Port));

                var messageContent = new StringContent(content);
                var response = await client.PostAsync(new Uri(String.Format("http://{0}:{1}", IPAddress, Port), UriKind.Absolute), messageContent);
                Debug.WriteLine("RESPONSE CODE: " + response.StatusCode);
                Debug.WriteLine("RESPONSE CONTENT: " + response.Content);
                Debug.WriteLine("--------------------------------------------------------------");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION: " + ex.Message);
                Debug.WriteLine("--------------------------------------------------------------");
                return false;
            }
        }
    }

    public class SmartThingsHubs
    {
        private List<SmartThingsHub> _hubs;
        private static SmartThingsHubs _instance = new SmartThingsHubs();

        private const string HUBS_JSON = "HUBS_JSON";

        DateTime? _lastPing;

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private SmartThingsHubs()
        {

        }

        public static SmartThingsHubs Instance { get { return _instance; } }

        public async Task InitAsync()
        {
            var json = await PlatformSupport.Services.Storage.GetKVPAsync<String>(HUBS_JSON);
            if (String.IsNullOrEmpty(json))
                _hubs = new List<SmartThingsHub>();
            else
                _hubs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SmartThingsHub>>(json);

            _isInitialized = true;
        }

        public void Ping(String hubId)
        {
            var stHub = _hubs.Where(hub => hub.Id == hub.Id).FirstOrDefault();
            if (stHub != null)
                stHub.LastPing = DateTime.Now;

            _lastPing = DateTime.Now;
        }

        public DateTime? LastPing
        {
            get { return _lastPing; }
        }

        public async void Save()
        {
            await PlatformSupport.Services.Storage.StoreKVP(HUBS_JSON, JsonConvert.SerializeObject(_hubs));
        }

        public void Prune()
        {

        }

        public List<SmartThingsHub> Hubs
        {
            get
            {
                if (_hubs == null)
                    throw new Exception("Must initialize SmartThingsHubs service prior to accessing it.");

                return _hubs;
            }
        }

        public async Task SendToHubsAsync(String content)
        {
            foreach (var stHub in _hubs)
                await stHub.SendAsync(content);
        }

    }
}
