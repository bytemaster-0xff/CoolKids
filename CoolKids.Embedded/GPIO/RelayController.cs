using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
            try
            {
                IsDemoMode = String.IsNullOrEmpty(i2cDeviceId);
                if (!IsDemoMode)
                {
                    var settings = new I2cConnectionSettings(slaveAddress)
                    {
                        BusSpeed = I2cBusSpeed.FastMode,
                        SharingMode = I2cSharingMode.Shared
                    };
                 



                    string aqs = I2cDevice.GetDeviceSelector(i2cDeviceId);                       /* Find the selector string for the I2C bus controller                   */
                    var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the I2C bus controller device with our selector string           */
                    _relayControllerChannel = await I2cDevice.FromIdAsync(dis[0].Id, settings);    /* Create an I2cDevice with our selected bus controller and I2C settings */





                }
            }
            catch (Exception e)
            {

                Debug.WriteLine("Init on RelayController failed");
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
                    try
                    {
                        relayOn = value;

                        byte[] WriteBuf;

                        if (relayOn)
                        {

                            WriteBuf = new byte[] { 0x00 }; /* Some data to write to the device */
                        }
                        else
                        {

                            WriteBuf = new byte[] { 0x01 }; /* Some data to write to the device */
                        }



                        _relayControllerChannel.Write(WriteBuf);


                      //  byte[] ReadBuf = new byte[] { 0x00 };

                   //     _relayControllerChannel.Read(ReadBuf);


                    } 
                    catch
                    {

                        Debug.WriteLine("Write failed");
                    }
                }
            }
        }
    }
}
