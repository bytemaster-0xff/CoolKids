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
using System.Net.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolKids.Uwp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		Camera.Models.Camera Cam1 { get; set; }
		bool Cam1IsRunning = false;
		int CurrentTemp { get; set; }
		int DefaultTemp = 75;
		//DateTime? ParentLastSeen { get; set; }
		bool AlarmHasBeenSent = false;

		System.Threading.Timer UpdateTimer;

		public MainPage()
		{
			this.InitializeComponent();
			Cam1 = Camera.Models.Camera.Create();

			CurrentTemp = DefaultTemp;
			SetupAutomation();

			//	Loaded += MainPage_Loaded;
			//}

			//private async void MainPage_Loaded(object sender, RoutedEventArgs e)
			//{
			//	await GetImage();
			//	var cogResult = await ComputerVisionApi.Detect(Cam1.CurrentBytes);
			//	if (Cam1.CurrentPicture != null)
			//	{
			//		Cam1Image.Source = Cam1.CurrentPicture;
			//	}
		}

		private async void SetupAutomation()
		{
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				M2xResult.Text = string.Empty;
				Cam1Result.Text = "Started";
			});

			if (!Cam1IsRunning)
			{
				UpdateTimer = new System.Threading.Timer(UpdateTimerCallback, null, 0, 3250);
				Cam1IsRunning = true;
			}
		}

		private async void UpdateTimerCallback(object state)
		{
			await GetImage();
		}

		private async Task GetImage()
		{
			Cam1.Url = $"192.168.1.201";
			Cam1.Port = 80;
			Cam1.UserName = "coolkids";
			Cam1.Password = "tampa";
			await Cam1.DownloadImage();

			if (Cam1.CurrentBytes != null)
			{
				var m2x = new M2xApi();

				// cog services request
				var cogResult = await ComputerVisionApi.Detect(Cam1.CurrentBytes);
				//if (ParentLastSeen.HasValue && !cogResult.ToLowerInvariant().Contains("people"))
				if (!cogResult.ToLowerInvariant().Contains("people"))
				{
					if (!AlarmHasBeenSent)
					{
						AlarmHasBeenSent = true;

						var hc = new HttpClient();

						await hc.GetStringAsync("http://192.168.1.8:9001?01");
						await hc.GetStringAsync("http://192.168.1.8:9001?11");

						await Task.Delay(2000);

						await hc.GetStringAsync("http://192.168.1.8:9001?00");
						await hc.GetStringAsync("http://192.168.1.8:9001?10");

						await m2x.PostValue("alarm_condition", "true");
					}
				}
				else if (cogResult.ToLowerInvariant().Contains("people"))
				{
					await m2x.PostValue("alarm_condition", "false");
					await m2x.PostValue(M2xApi.TemperatureType.Interior, CurrentTemp);
				}
				else
				{
					//ParentLastSeen = DateTime.Now;
					await m2x.PostValue("alarm_condition", "false");
					await m2x.PostValue(M2xApi.TemperatureType.Interior, CurrentTemp);
				}

				await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
					Cam1Result.Text = $"{DateTime.Now.Ticks} - {cogResult}";
				});
			}

			//if (Cam1.CurrentPicture != null)
			//{
			//	Cam1Image.Source = Cam1.CurrentPicture;
			//}
		}

		private async void RefreshButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			SetupAutomation();
			await GetImage();
		}

		private async void SetInteriorTemperature(object sender, TappedRoutedEventArgs e)
		{
			var m2x = new M2xApi();
			var result = "no change";

			var b = (Button)sender;
			switch ((string)b.Tag)
			{
				case "Increment05":
					CurrentTemp += 5;
					result = await m2x.PostValue(M2xApi.TemperatureType.Interior, CurrentTemp);
					break;
				case "Increment10":
					CurrentTemp += 10;
					result = await m2x.PostValue(M2xApi.TemperatureType.Interior, CurrentTemp);
					break;
				case "Reset":
					CurrentTemp = DefaultTemp;
					result = await m2x.PostValue(M2xApi.TemperatureType.Interior, CurrentTemp);
					SetupAutomation();
					break;
				case "Stop":
					KillAutomation(sender, null);
					break;
			}

			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				M2xResult.Text = $"{DateTime.Now.Ticks} - {result}";
			});
		}

		private async void KillAutomation(object sender, TappedRoutedEventArgs e)
		{
			Cam1IsRunning = false;
			UpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				Cam1Result.Text = "Killed";
			});
		}

		private async void GetcarLocation(object sender, TappedRoutedEventArgs e)
		{
			var m2x = new M2xApi();
			var result = await m2x.GetCarLocation();
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				M2xResult.Text = $"{DateTime.Now.Ticks} - {result}";
			});
		}
	}
}
