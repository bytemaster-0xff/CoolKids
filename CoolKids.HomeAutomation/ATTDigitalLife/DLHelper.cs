using CoolKids.HomeAutomation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation
{
    public class DLHelper
    {

        private DL_Device devices;
        private DLClient client;

        private string userName;
        private string password;
        private string apiKey;

        public DLHelper(string userName, string password, string apiKey)
        {
            this.userName = userName;
            this.password = password;
            this.apiKey = apiKey;

        }


        private async Task LoadDevices()
        {
            if (devices == null)
            {
                string host = "https://systest.digitallife.att.com:443";

                client = new DLClient(host, userName, password, apiKey);

                client.InitializationTask.Wait();

                devices = await client.listAllDevices();
            }
        }

        public async Task<string> ToggleLights()
        {
            await LoadDevices();

            string deviceGuid = "";
            string deviceGuid_smartplug = "";

            foreach (var item in devices.content)
            {
                if (item.deviceType == "light-control")
                {
                    deviceGuid = item.deviceGuid;
                }
                else if (item.deviceType == "smart-plug")
                {
                    deviceGuid_smartplug = item.deviceGuid;
                }
            }

            string onoff = "off";

            if (String.IsNullOrWhiteSpace(deviceGuid) == false)
            {

                DL_DeviceAttribute attribute = await client.getDeviceAttributeValue(deviceGuid, "switch");

                if (attribute.content.value == "off")
                    onoff = "on";

                DL_Status result = await client.updateDeviceAttributeValue(deviceGuid, "switch", onoff);
            }


            if (String.IsNullOrWhiteSpace(deviceGuid_smartplug) == false)
            {
                DL_Status result = await client.updateDeviceAttributeValue(deviceGuid_smartplug, "switch", onoff);
            }

            return onoff;
        }


        public async Task<string> LockOpenClose(bool Unlock = true)
        {
            await LoadDevices();

            string deviceGuid_doorlock1 = "";

            string lockUnlock = (Unlock ? "unlock" : "lock" );

            foreach (var item in devices.content)
            {
                if (item.deviceType == "door-lock" &&
                    String.IsNullOrWhiteSpace(deviceGuid_doorlock1) == true)
                {
                    deviceGuid_doorlock1 = item.deviceGuid;
                    break;
                }
            }

            DL_Status result = await client.updateDeviceAttributeValue(deviceGuid_doorlock1, "lock", lockUnlock);

            return result.content;
        }


        public async Task<string> ATT_Alarm(bool disable = true)
        {
            await LoadDevices();

            string state = (disable ? "home" : "away");

            DL_Status result = await client.updateAlarmState(state);

            DL_Status result2 = await client.getAlarmState();

            return result2.content;            
        }

        public void ResetCredentials(string userName, string password, string apiKey)
        {
            this.userName = userName;
            this.password = password;
            this.apiKey = apiKey;
        
            devices = null;
        }

    }
}
