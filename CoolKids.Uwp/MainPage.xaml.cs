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
using CoolKids.Services.Att;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolKids.Uwp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		Camera.Models.Camera Cam0 { get; set; }
		Camera.Models.Camera Cam1 { get; set; }
		Camera.Models.Camera Cam2 { get; set; }
		Camera.Models.Camera Cam3 { get; set; }
		Camera.Models.Camera Cam4 { get; set; }
		Camera.Models.Camera Cam5 { get; set; }

		public MainPage()
		{
			this.InitializeComponent();
			Loaded += MainPage_Loaded;
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			Cam0 = Camera.Models.Camera.Create();
			Cam1 = Camera.Models.Camera.Create();
			Cam2 = Camera.Models.Camera.Create();
			Cam3 = Camera.Models.Camera.Create();
			Cam4 = Camera.Models.Camera.Create();
			Cam5 = Camera.Models.Camera.Create();
			await GetImage();
		}

		private async Task GetImage()
		{
			foreach (var pair in new Dictionary<string, Tuple<Camera.Models.Camera, Image, TextBox>> {
				{ "200", Tuple.Create(Cam0, Cam0Image, Cam0Result) },
				{ "201", Tuple.Create(Cam1, Cam1Image, Cam1Result) },
				{ "202", Tuple.Create(Cam2, Cam2Image, Cam2Result) },
				{ "203", Tuple.Create(Cam3, Cam3Image, Cam3Result) },
				{ "204", Tuple.Create(Cam4, Cam4Image, Cam4Result) },
				{ "205", Tuple.Create(Cam5, Cam5Image, Cam5Result) },
			})
			{
				pair.Value.Item1.Url = $"192.168.1.{pair.Key}";
				pair.Value.Item1.Port = 80;
				pair.Value.Item1.UserName = "coolkids";
				pair.Value.Item1.Password = "tampa";
				await pair.Value.Item1.DownloadImage();

				var check = pair.Value.Item1.CurrentPicture;
				if (check != null)
				{
					pair.Value.Item2.Source = check;

					// cog services request
					await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
					{
						pair.Value.Item3.Text = await FaceApi.Detect(Cam1.CurrentBytes);
					});
				}
			}
		}

		private async void RefreshButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			await GetImage();
		}

		private void Cam1Result_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}

		private async void SetDoorStatus(object sender, TappedRoutedEventArgs e)
		{
			var m2x = new M2xApi();
			var result = "no change";

			var b = (Button)sender;
			switch ((string)b.Tag)
			{
				case "DoorDriverAjar":
					result = await m2x.PostValue(M2xApi.CarDoorType.DriverRear, M2xApi.CarDoorValue.Ajar);
					break;
				case "DoorDriverClosed":
					result = await m2x.PostValue(M2xApi.CarDoorType.DriverRear, M2xApi.CarDoorValue.Closed);
					break;
				case "DoorDriverOpen":
					result = await m2x.PostValue(M2xApi.CarDoorType.DriverRear, M2xApi.CarDoorValue.Open);
					break;
				case "DoorPassengerAjar":
					result = await m2x.PostValue(M2xApi.CarDoorType.PassengerRear, M2xApi.CarDoorValue.Ajar);
					break;
				case "DoorPassengerClosed":
					result = await m2x.PostValue(M2xApi.CarDoorType.PassengerRear, M2xApi.CarDoorValue.Closed);
					break;
				case "DoorPassengerOpen":
					result = await m2x.PostValue(M2xApi.CarDoorType.PassengerRear, M2xApi.CarDoorValue.Open);
					break;
			}
		}

		private async void SetWindowStatus(object sender, TappedRoutedEventArgs e)
		{
			var m2x = new M2xApi();
			var result = "no change";

			var b = (Button)sender;
			switch ((string)b.Tag)
			{
				case "WindowDriverRearClosed":
					result = await m2x.PostValue(M2xApi.CarWindowType.DriverRear, 0);
					break;
				case "WindowDriverRearOpen":
					result = await m2x.PostValue(M2xApi.CarWindowType.DriverRear, 100);
					break;
				case "WindowPassengerRearClosed":
					result = await m2x.PostValue(M2xApi.CarWindowType.PassengerRear, 0);
					break;
				case "WindowPassengerRearOpen":
					result = await m2x.PostValue(M2xApi.CarWindowType.PassengerRear, 100);
					break;
			}
		}
	}
}
