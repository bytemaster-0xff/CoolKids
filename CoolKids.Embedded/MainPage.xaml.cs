using CoolKids.HomeAutomation;
using CoolKids.Uwp.Embedded;
using CoolKids.Uwp.Embedded.GPIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        private CoolKidsHTTPServer webserver;

        private string appId = "EE_B4BEFBF683DB0144_1";
        private string password = "NO-PASSWD";
        private bool sanFranEventFired = false;

        public Services.Att.M2xApi m2x { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            motionDetector = new Motion();
            motionDetector.InitGPIO();
            motionDetector.Changed += MotionDetector_Changed;

            motionDetector.Start();

            _main = new Uwp.Embedded.Services.Main();


            Loaded += MainPage_Loaded;

            webserver = new CoolKidsHTTPServer();
            webserver.StartServer();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _device = new ArduinoDevice();
            await _device.Init(0x60, "I2C0");

            _main.Port = 9001;
            _main.MessageReceived += _main_MessageReceived;
            _main.StartServer();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            UN.Text = "553474447";  //Igloo8

        }

        private async void _timer_Tick(object sender, object e)
        {
            //var temp = _device.ReadTemperature();
            //Debug.WriteLine("TEMP: " + temp);

            if (m2x == null)
            {
                m2x = new Services.Att.M2xApi();
            }

            var location = await m2x.GetCarLocation();

            M2xLocationResponse m2xloc = JSONSerializationHelper.Deserialize<M2xLocationResponse>(location);

             if (m2xloc != null)
            {
                HttpClient client = new HttpClient();

                string url = $"https://api.opencagedata.com/geocode/v1/json?query={m2xloc.latitude},{m2xloc.longitude}&pretty=1&key=fd953ae4c5c3c3b259c0aa35ebdce2a4";

                string response = await client.GetStringAsync(url);

                bool isNearSanFran = response.ToString().ToLower().Contains("san francisco");

                if (isNearSanFran &&
                    sanFranEventFired == false)
                {
                    // We are new San Francisco, fire off event.

                    var urlToCall = "http://192.168.1.9:8080/itv/startURL?url=http://192.168.1.2:8000";

                    await client.GetAsync(urlToCall);

                    sanFranEventFired = true;
                }
            }
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

        }


        private async void btn_LeaveHome(object sender, RoutedEventArgs e)
        {
            DLHelper dl = new DLHelper(UN.Text, password, appId);

            await dl.LoadDevices();

            Task t1 = dl.ATT_Alarm(false);
            Task t2 = dl.LockOpenClose(false);
            Task t3 = dl.TurnOnLights(false);

            await Task.WhenAll(t1, t2, t3);

        }

        private async void btn_ArriveAtHome(object sender, RoutedEventArgs e)
        {
            DLHelper dl = new DLHelper(UN.Text, password, appId);

            await dl.LoadDevices();

            Task t1 = dl.ATT_Alarm(true);
            Task t2 = dl.LockOpenClose(true);
            Task t3 = dl.TurnOnLights(true);

            await Task.WhenAll(t1, t2, t3);
        }
    }



    public class M2xLocationResponse
    {
        public string name { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string elevation { get; set; }
        public string timestamp { get; set; }

    }





}
