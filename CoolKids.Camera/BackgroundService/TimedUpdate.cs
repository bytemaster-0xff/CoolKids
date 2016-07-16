using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.BackgroundService
{
	public class TimedUpdate
	{
		public async Task Run()
		{
			if (Common.PlatformSupport.Services.Network.IsInternetConnected)
			{
				await CameraDataServices.Instance.InitAsync();
				await CameraDataServices.Instance.RefreshCameraSnapshotsAsync();
			}
		}
	}
}