using CoolKids.Uwp.Embedded.GPIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolKids.Embedded
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Motion motionDetector;

        private ArduinoDevice _device;

        CoolKids.Uwp.Embedded.Services.Main _main;
              
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();

            motionDetector = new Motion();
            motionDetector.InitGPIO();
            motionDetector.Changed += MotionDetector_Changed;

            motionDetector.Start();

            _main = new Uwp.Embedded.Services.Main();
            
         
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _device = new ArduinoDevice();
            await _device.Init(0x60, "I2C0");

            _main.Port = 9001;
            _main.MessageReceived += _main_MessageReceived;
            _main.StartServer();

           /* _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += _timer_Tick;
            _timer.Start();*/

        }

        private void _timer_Tick(object sender, object e)
        {
            var temp = _device.ReadTemperature();
            Debug.WriteLine("TEMP: " + temp);
        }

        private void _main_MessageReceived(object sender, string e)
        {
            Debug.WriteLine("MESSAGE RECEIVED Forward -->     " + e);

            _device.Send(e);
        }

 
        private void MotionDetector_Changed(bool MotionDetected)
        {
            if (MotionDetected)
            {
                tbMessage.Text = "Motion DETECTED";
            }
            else
            {
                tbMessage.Text = "Motion not detected";
            }
            //throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         }
    }
}
