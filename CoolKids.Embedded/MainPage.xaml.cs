using CoolKids.HomeAutomation;
using CoolKids.Uwp.Embedded;
using CoolKids.Uwp.Embedded.GPIO;
using System;
using System.Collections.Generic;
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

        private Relay relay1;

        private RelayController relayController1;

        private DispatcherTimer timer;

        private CoolKidsHTTPServer webserver;

        private string appId = "EE_B4BEFBF683DB0144_1";
        private string password = "NO-PASSWD";

        public MainPage()
        {
            this.InitializeComponent();

            motionDetector = new Motion();
            motionDetector.InitGPIO();
            motionDetector.Changed += MotionDetector_Changed;

            motionDetector.Start();
            
            relay1 = new Relay();
            relay1.InitGPIO();

            Loaded += MainPage_Loaded;

            webserver = new CoolKidsHTTPServer();
            webserver.StartServer();


        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            relayController1 = new RelayController();
            await relayController1.Init("I2C1", 0x30);


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += Timer_Tick;
            timer.Start();

            UN.Text = "553474447";  //Igloo8

        }

        private void Timer_Tick(object sender, object e)
        { 
        
         //   relay1.IsRelayOn = !relay1.IsRelayOn;

       

            relayController1.IsRelayOn = !relayController1.IsRelayOn;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            relay1.IsRelayOn = !relay1.IsRelayOn;


            relayController1.IsRelayOn = !relayController1.IsRelayOn;
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
}
