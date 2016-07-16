using CoolKids.Camera.Models;
using CoolKids.Common.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.Services
{
	public class CameraService
	{
		IStorageService _persistence;

		const string CAMERA_API_FILE_NAME = "CameraAPIConfig.dat";
		const string CAMERA_FILE_NAME = "camera.dat";
		const string CAMERA_SETTINGS_FILE_NAME = "settings.dat";

		private CameraService() { }

		public CameraService(IStorageService persistence)
		{
			_persistence = persistence;
		}


		public const String STR_GET_DEV_INFO = "getDevInfo";
		public const String STR_GET_DEV_STATE = "getDevState";
		public const String STR_GET_VIDEO_STREAM = "getVideoStreamParam";
		public const String STR_GET_SUB_VIDEO_STREAM = "getSubVideoStreamParam";
		public const String STR_GET_DEFRAME_LEVEL = "getDeFrameLevel";

		public const string STR_ACTION_MOVEEND = "ptzStopRun";
		public const string STR_ACTION_UP = "ptzMoveUp";
		public const string STR_ACTION_DOWN = "ptzMoveDown";
		public const string STR_ACTION_LEFT = "ptzMoveLeft";
		public const string STR_ACTION_RIGHT = "ptzMoveRight";
		public const string STR_ACTION_FLIPVIDEO = "flipVideo";   //&isFlip=0
		public const string STR_ACTION_MIRRORVIDEO = "mirrorVideo";   //&isMirror=0


		private const String STR_SetMJPEGStream = "http://{1}:{1}/cgi-bin/CGIProxy.fcgi?cmd=setSubStreamFormat&format=1&usr={2}&pwd={3}";


		public async Task SaveCameraAPIConfig(CameraAPIConfigs settings)
		{
			var ser = new DataContractJsonSerializer(typeof(CameraAPIConfigs));

			using (var ms = new MemoryStream())
			{
				ser.WriteObject(ms, settings);
				ms.Seek(0, SeekOrigin.Begin);

				var strReader = new StreamReader(ms);
				var camOut = strReader.ReadToEnd();

				Debug.WriteLine(camOut);
				ms.Seek(0, SeekOrigin.Begin);
				await _persistence.StoreAsync(ms, Locations.Roaming, CAMERA_API_FILE_NAME);
			}
		}

		public async Task<CameraAPIConfigs> GetCameraAPIConfigs()
		{
			using (var ms = await _persistence.Get(Locations.Roaming, CAMERA_API_FILE_NAME))
			{
				if (ms == null)
				{
					var apiConfig = new CameraAPIConfigs()
					{
						Version = 1.0,
						DateStamp = new DateTime(2013, 10, 18)
					};

					apiConfig.Configs = new List<CameraAPIConfig>();
					apiConfig.Configs.Add(CameraTypes.GetCGI());
					apiConfig.Configs.Add(CameraTypes.GetCGI2());
					apiConfig.Configs.Add(CameraTypes.GetDigestCGI());
					apiConfig.Configs.Add(CameraTypes.GetFoscamHD());
					apiConfig.Configs.Add(CameraTypes.GetAMCrest());


					return apiConfig;
				}
				else
				{
					var ser = new DataContractJsonSerializer(typeof(CameraAPIConfigs));
					return ser.ReadObject(ms) as Models.CameraAPIConfigs;
				}
			}
		}


		public async Task<Models.CameraAPIConfigs> GetFromServer()
		{
			var client = new HttpClient();

			var ser = new DataContractJsonSerializer(typeof(CameraAPIConfigs));

			using (var stream = await client.GetStreamAsync("http://bytemaster.blob.windowazure.com/ipcamultimate/cameraapi.dat"))
			{
				await _persistence.StoreAsync(stream, Locations.Roaming, CAMERA_API_FILE_NAME);
			}

			return await GetCameraAPIConfigs();
		}

		public async Task<Models.CameraSettings> GetCameraSettings()
		{
			var ser = new DataContractJsonSerializer(typeof(Models.CameraSettings));

			try
			{
				using (var fileStream = await _persistence.Get(Locations.Roaming, CAMERA_SETTINGS_FILE_NAME))
				{
					if (fileStream != null && fileStream.Length > 0)
					{
						var strReader = new StreamReader(fileStream);
						Debug.WriteLine(strReader.ReadToEnd());
						fileStream.Seek(0, SeekOrigin.Begin);

						return ser.ReadObject(fileStream) as Models.CameraSettings;
					}
					else
					{
						return new CameraSettings()
						{
							DefaultUserName = String.Empty,
							DefaultPassword = String.Empty,
							DefaultUrl = String.Empty,
							ViewPassword = false,
							IsSavingSettings = null
						};
					}
				}
			}
			catch (Exception ex)
			{
				return new CameraSettings()
				{
					DefaultUserName = String.Empty,
					DefaultPassword = String.Empty,
					DefaultUrl = String.Empty,
					ViewPassword = false,
					IsSavingSettings = null
				};
			}
		}

		public async Task SaveCameraSettings(CameraSettings settings)
		{
			var ser = new DataContractJsonSerializer(typeof(CameraSettings));

			using (var ms = new MemoryStream())
			{
				ser.WriteObject(ms, settings);
				ms.Seek(0, SeekOrigin.Begin);

				var strReader = new StreamReader(ms);
				var camOut = strReader.ReadToEnd();

				Debug.WriteLine(camOut);
				ms.Seek(0, SeekOrigin.Begin);

				await _persistence.StoreAsync(ms, Locations.Roaming, CAMERA_SETTINGS_FILE_NAME);
			}
		}

		public async Task<List<Models.Camera>> GetCameras()
		{
			var ser = new DataContractJsonSerializer(typeof(List<Models.Camera>));

			using (var fileStream = await _persistence.Get(Locations.Roaming, CAMERA_FILE_NAME))
			{
				if (fileStream != null && fileStream.Length > 0)
				{
					return ser.ReadObject(fileStream) as List<Models.Camera>;
				}
				else
					return new List<Models.Camera>();
			}
		}

		public async Task SaveCameras(List<Models.Camera> cameras)
		{
			var ser = new DataContractJsonSerializer(typeof(List<Models.Camera>));

			using (var ms = new MemoryStream())
			{
				ser.WriteObject(ms, cameras);
				ms.Seek(0, SeekOrigin.Begin);

				var strReader = new StreamReader(ms);
				var camOut = strReader.ReadToEnd();

				Debug.WriteLine(camOut);
				ms.Seek(0, SeekOrigin.Begin);

				await _persistence.StoreAsync(ms, Locations.Roaming, CAMERA_FILE_NAME);
			}
		}
	}
}