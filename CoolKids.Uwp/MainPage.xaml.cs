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
using CoolKids.Services.MicrosoftCognitiveServices;
using System.Threading.Tasks;

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
			await GetImage();
		}

		private async Task GetImage()
		{
			Cam1 = Camera.Models.Camera.Create();
			Cam1.Url = "192.168.1.201";
			Cam1.Port = 80;
			Cam1.UserName = "coolkids";
			Cam1.Password = "tampa";
			await Cam1.DownloadImage();

			var check = Cam1.CurrentPicture;
			if (check != null)
			{
				Cam1Image.Source = check;

				// cog services request
				await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
				{
					Cam1Result.Text = await FaceApi.Detect(Cam1.CurrentBytes);
				});
			}
		}

		private async void RefreshButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			await GetImage();
		}

		private void Cam1Result_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}
	}
}
