using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace CoolKids.Uwp.Embedded.GPIO
{
 
    public class Relay
    {
        private int RELAY_PIN;

        private GpioPin pin = null;        

        private bool relayOn = false;

        public bool IsRelayOn
        {
            get { return relayOn; }
            set
            {
                if (pin != null)
                {
                    relayOn = value;

                    if (relayOn == true)
                    {
                        pin.Write(GpioPinValue.High);
                    }
                    else
                    {
                        pin.Write(GpioPinValue.Low);
                    }
                }
            }
        }

        ~Relay()
        {

            if (pin != null)
            {
                pin.Dispose();
            }
        }

        public void InitGPIO(int relaypin = 33)
        {

            RELAY_PIN = relaypin;

            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                Debug.WriteLine("There is no GPIO controller on this device.");
                return;

            }
            pin = gpio.OpenPin(RELAY_PIN);

            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            Debug.WriteLine("GPIO pin initialized correctly.");
        }


    }



}
