using CoolKids.HomeAutomation.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation
{
    public class DLClient
    {
        public String gatewayGUID { get; private set; }
        public String host { get; private set; }
        public String apiKey { get; private set; }

        public Task InitializationTask {get; set;}
        
        private String authToken = null;
        private String requestToken = null;

        public DLClient(string host, string userName, string password, string apiKey)
        {
            this.host = host;
            this.apiKey = apiKey;

            InitializationTask = Task.Run(async () =>
            {
                string baseURL = "/penguin/api/authtokens";
                string parameters = String.Format("?userId={0}&password={1}&domain={2}&appKey={3}&rememberMe=false",
                                                userName,
                                                password,
                                                "DL",
                                                apiKey);
                
                string data = await fetchData(baseURL + parameters, "");

                JObject auth = JObject.Parse(data);

                gatewayGUID = auth["content"]["gateways"][0]["id"].ToString();
                requestToken = auth["content"]["requestToken"].ToString();
                authToken = auth["content"]["authToken"].ToString();
            });
        }
        
        public async Task<long> updateDevice(String deviceGUID, String action, String value)
        {
            string url = "/penguin/api/{gatewayGUID}/devices/" + deviceGUID + "/" + action + "/" + value;
            string result = await fetchData(url, "");
            long returnvalue = (long)(JObject.Parse(result)["content"]);

            return returnvalue;
        }

        public async Task<DL_Device> listAllDevices()
        {
            string url = "/penguin/api/{gatewayGUID}/devices";
            string result = await fetchData(url);

            return JSONSerializationHelper.Deserialize<DL_Device>(result);
        }


        public async Task<DL_DeviceAttribute> getDeviceAttributeValue(string deviceGuid, string attributeName )
        {
            string url = "/penguin/api/{gatewayGUID}/";
            string fragment = String.Format("devices/{0}/{1}", deviceGuid, attributeName);
            string result = await fetchData(url+fragment);

            return JSONSerializationHelper.Deserialize<DL_DeviceAttribute>(result);
        }


        public async Task<DL_Status> updateDeviceAttributeValue(string deviceGuid, string attributeName, string value)
        {
            string url = "/penguin/api/{gatewayGUID}/";
            string fragment = String.Format("devices/{0}/{1}/{2}", deviceGuid, attributeName, value);
            string result = await fetchData(url + fragment, "");

            return JSONSerializationHelper.Deserialize<DL_Status>(result);
        }


        public async Task<DL_Status> getAlarmState()
        {
            string url = "/penguin/api/{gatewayGUID}/alarm";

            string result = await fetchData(url);

            return JSONSerializationHelper.Deserialize<DL_Status>(result);
        }

        public async Task<DL_Status> updateAlarmState(string state)
        {
            string url = "/penguin/api/{gatewayGUID}/alarm";

            DL_StatusBypass sb = new DL_StatusBypass() { status = "home" };
            string postbody = JSONSerializationHelper.Serialize<DL_StatusBypass>(sb);

            string result = await fetchData(url, postbody);

            return JSONSerializationHelper.Deserialize<DL_Status>(result);
        }

        public async Task<String> fetchData(string realitiveURL, 
                                            string postBody = null)
        {
            if (String.IsNullOrWhiteSpace(gatewayGUID) == false)
                realitiveURL = realitiveURL.Replace("{gatewayGUID}", gatewayGUID);
   
            string fullURL = host + realitiveURL;

       
            if (authToken != null)
            {
                ATTDigitalLifeHttpHelper.authToken = authToken;
                ATTDigitalLifeHttpHelper.requestToken = requestToken;
                ATTDigitalLifeHttpHelper.appKey = apiKey;
            }
            
            if (postBody == null)
            {
                //Assume Get
                return await ATTDigitalLifeHttpHelper.GetAsync(fullURL, "", true);
            }
            else if (postBody != null)
            {
                var content = new StringContent(postBody, Encoding.UTF8, "application/json");
                return await ATTDigitalLifeHttpHelper.PostAsync(fullURL, "", content);
            }

            return "";
        }
    }
}
