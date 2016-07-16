using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation
{
    public class DeviceJSONWrapper
    {
        private JObject deviceJSON;

        public DeviceJSONWrapper(JObject deviceJSON)
        {
            this.deviceJSON = deviceJSON;
        }

        public List<JToken> getDevicesByClass(String deviceTypeName)
        {
            return (from device in deviceJSON["content"]
                    where device["deviceType"].ToString() == deviceTypeName
                    select device).ToList();
        }

        public bool isDeviceOffline(String deviceGUID)
        {
            return getDeviceAttribute(deviceGUID, "status") == "2";
        }

        public bool isDeviceOffline(JToken device)
        {
            return getDeviceAttribute(device, "status") == "2";
        }

        public bool hasAttribute(JToken device, String attributeName)
        {
            return (from attribute in device["attributes"]
                    where attribute["label"].ToString() == attributeName
                    select attribute).Count()>0;
        }

        public String getDeviceAttribute(String deviceGUID, String attributeName)
        {
            return getDeviceAttribute(getDevice(deviceGUID), attributeName);
        }

        public JToken getDevice(String deviceGUID)
        {
            return (from device in deviceJSON["content"]
                    where device["deviceGuid"].ToString() == deviceGUID
                    select device).First();
        }

        public String getDeviceAttribute(JToken device, String attributeName)
        {
            try
            {
                return (from attribute in device["attributes"]
                        where attribute["label"].ToString() == attributeName
                        select attribute["value"]).First().ToString();
            }
            catch
            {
                //You quereied for a attribute that does not exist
                return null;
            }
        }

        public String getDeviceName(String deviceGUID)
        {
            return getDeviceAttribute(deviceGUID, "name").ToString();
        }

        public String getDeviceName(JToken device)
        {
            return getDeviceAttribute(device, "name").ToString();
        }

        //After the inital Query, this function keep add the data uptodate in real time.
        public void eServiceEvent(DLClient client, String message)
        {
            try
            {
                JObject eServiceMessage = JObject.Parse(message);
                if (eServiceMessage["type"] != null && eServiceMessage["type"].ToString() == "device")
                {
                    JToken device = getDevice(eServiceMessage["dev"].ToString());
                    JToken attribue = (from attribute in device["attributes"]
                                       where attribute["label"].ToString() == eServiceMessage["label"].ToString()
                                       select attribute).First();
                    ((JObject)attribue)["value"] = eServiceMessage["value"].ToString();
                }
            }catch{}
        }
    }
}
