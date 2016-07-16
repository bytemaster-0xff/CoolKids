using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace CoolKids.Uwp.Embedded.GPIO
{
    public class RelayController
    {
        Windows.Devices.I2c.I2cDevice _relayControllerChannel;
        bool IsDemoMode = false;

        public RelayController()
        {
        }

        public async Task Init(string i2cDeviceId = "I2C0", int slaveAddress = 0x30)
        {
            IsDemoMode = String.IsNullOrEmpty(i2cDeviceId);
            if (!IsDemoMode)
            {
                var settings = new I2cConnectionSettings(slaveAddress)
                {
                    BusSpeed = I2cBusSpeed.StandardMode,
                    SharingMode = I2cSharingMode.Shared
                };
                _relayControllerChannel = await Windows.Devices.I2c.I2cDevice.FromIdAsync(i2cDeviceId, settings);
            }
        }

        private bool relayOn = false;

        public bool IsRelayOn
        {
            get { return relayOn; }
            set
            {
                if (!IsDemoMode &&
                    _relayControllerChannel != null)
                {
                    relayOn = value;

                    byte[] values = new byte[] { (relayOn ? (byte)0x01 : (byte)0x00) };

                    _relayControllerChannel.Write(values);
                }
            }
        }
    }
}
