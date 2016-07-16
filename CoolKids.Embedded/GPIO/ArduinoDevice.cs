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
    public class ArduinoDevice
    {
        Windows.Devices.I2c.I2cDevice _channel;


        public async Task Init(byte slaveAddress, String i2cDeviceId = "I2C0")
        {
            try
            {
                var settings = new I2cConnectionSettings(slaveAddress)
                {
                    BusSpeed = I2cBusSpeed.FastMode,
                    SharingMode = I2cSharingMode.Shared
                };




                string aqs = I2cDevice.GetDeviceSelector(i2cDeviceId);                       /* Find the selector string for the I2C bus controller                   */
                var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the I2C bus controller device with our selector string           */
                _channel = await I2cDevice.FromIdAsync(dis[0].Id, settings);    /* Create an I2cDevice with our selected bus controller and I2C settings */
            }
            catch (Exception e)
            {
                Debug.WriteLine("Init on RelayController failed");
            }
        }

        public Int16 ReadTemperature()
        {
            try
            {
                var buffer = new byte[2] { 0x00, 0x00 };
                _channel.Read(buffer);
                return Convert.ToInt16((buffer[0] << 8) | buffer[1]);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("NOT CONNECTED");
                return -1;
            }
        }

        public bool Send(String msg)
        {
            try
            {
                var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(msg);

                _channel.Write(bytes);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Init on RelayController failed");
                return false;
            }
        }
    }
}
