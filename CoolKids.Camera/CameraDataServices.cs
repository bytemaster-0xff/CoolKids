using CoolKids.Camera.Models;
using CoolKids.Camera.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera
{
    public class CameraDataServices
    {
        private List<CoolKids.Camera.Models.Camera> _cameraData;
        private CameraSettings _cameraSettings;
        private CameraService _cameraService;
        private CameraAPIConfigs _cameraAPIConfigs;

        private bool _initialized = false;

        private CameraDataServices() { }

        private static CameraDataServices _instance = new CameraDataServices();
        public static CameraDataServices Instance { get { return _instance; } }

        public async Task InitAsync()
        {
            if (_initialized == false)
            {
                _cameraService = new CameraService(Common.PlatformSupport.Services.Storage);
                _cameraData = await _cameraService.GetCameras();
                _cameraSettings = await _cameraService.GetCameraSettings();
                _cameraAPIConfigs = await _cameraService.GetCameraAPIConfigs();
                _initialized = true;
            }
        }

        public List<CoolKids.Camera.Models.Camera> Cameras { get { return _cameraData; } }
        public CameraSettings CameraSettings { get { return _cameraSettings; } }

        public CameraAPIConfigs CameraAPIConfigs { get { return _cameraAPIConfigs;  } }

        public async Task RegreshCameraAPIConfigsFromServer()
        {
            await _cameraService.GetFromServer();
        }

        public async Task AddCamera(CoolKids.Camera.Models.Camera camera)
        {
            camera.IsNew = false;
            _cameraData.Add(camera);
            await _cameraService.SaveCameras(_cameraData);
        }

        public async Task DeleteCamera(CoolKids.Camera.Models.Camera camera)
        {
            _cameraData.Remove(camera);
            await _cameraService.SaveCameras(_cameraData);
        }

        public async Task SaveCameraSettings()
        {
            await _cameraService.SaveCameraSettings(_cameraSettings);
        }

        public async Task SaveCameraData()
        {
            await _cameraService.SaveCameras(_cameraData);
        }

        public async Task RefreshCameraSnapshotsAsync()
        {
            foreach (var camera in Cameras)
            {
                if (camera.UpdateInBackground)
                    await camera.RefreshCamera();
            }

            await SaveCameraData();
        }

        public static CoolKids.Camera.Models.Camera CreateNew()
        {
            var camera = CoolKids.Camera.Models.Camera.Create();
            camera.Index = Instance.Cameras.Count;
            camera.UserName = Instance.CameraSettings.DefaultUserName;
            camera.Url = Instance.CameraSettings.DefaultUrl;
            camera.Password = Instance.CameraSettings.DefaultPassword;
            camera.Id = Guid.NewGuid();

            return camera;
        }
    }
}
