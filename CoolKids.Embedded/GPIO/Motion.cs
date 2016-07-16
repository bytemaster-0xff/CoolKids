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
    public delegate void MotionDetectedEventHandler(bool MotionDetected);

    public class Motion
    {
        private int MOTION_PIN;
        
        private GpioPin pin = null;

        private DispatcherTimer timer;

        public event MotionDetectedEventHandler Changed;

        private bool MotionDetected = false;

        ~Motion()
        {
            if (pin != null)
            {
                pin.Dispose();
            }
        }

        public void InitGPIO(int motionpin = 36)
        {

            MOTION_PIN = motionpin;

            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                Debug.WriteLine("There is no GPIO controller on this device.");
                return;
            }

            pin = gpio.OpenPin(MOTION_PIN);
            pin.SetDriveMode(GpioPinDriveMode.Input);

            Debug.WriteLine("GPIO pin initialized correctly.");
        }

        public void Start()
        {
            if (pin != null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(500);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }


        private void Timer_Tick(object sender, object e)
        {
            var value = pin.Read().ToString();
            

            bool newMotionDetected = value == "High";

            if (newMotionDetected != MotionDetected)
            {
                Debug.WriteLine("GPIO pin value: " + value);

                MotionDetected = newMotionDetected;

                if (Changed != null)
                {
                    Changed(MotionDetected);
                }
            }

        }

    }
}
