using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CoolKids.Camera;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolKids.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		Camera.Models.Camera Cam1 { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
			Loaded += MainPage_Loaded;
        }

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			Cam1 = Camera.Models.Camera.Create();
			Cam1.Url = "192.168.1.201";
			Cam1.Port = 80;
			Cam1.UserName = "coolkids";
			Cam1.Password = "tampa";
			await Cam1.DownloadImage();

			Cam1Image.Source = Cam1.CurrentPicture;
		}
	}
}
