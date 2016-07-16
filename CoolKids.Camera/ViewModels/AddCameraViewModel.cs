using System;
using System.Collections.Generic;
using CoolKids.Camera.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolKids.Common.ViewModels;
using CoolKids.Common.IPCamera;
using CoolKids.Common.Commanding;
using CoolKids.Common.PlatformSupport;
using System.Diagnostics;

namespace CoolKids.Camera.ViewModels
{
    public class AddCameraViewModel : ViewModelBase
    {
        public enum ScreenStates
        {
            NetworkSettings,
            CameraPreview,
            AdvancedSettings,
            ConfirmSettings
        }

        ScreenStates ScreenState { get; set; }


        public async override Task InitAsync()
        {
            Camera = CameraDataServices.CreateNew();

            IsNextVisible = true;
            IsPrevVisible = false;
            IsSaveVisible = false;

            NetworkSettingsVisible = true;
            CameraPreviewVisible = false;
            AdvancedSettingsVisible = false;
            FinishVisible = false;


            ScreenState = ScreenStates.NetworkSettings;

            await Task.Delay(1);
        }

        private async Task<Boolean?> TestCamera()
        {
            if (!IsNetworkConnected)
            {
                await Popups.ShowAsync(CoolKidsCameraCommon.NoInternetMessage);
                return null;
            }

            if (Camera.Port == 0)
            {
                await Popups.ShowAsync(CoolKidsCameraCommon.PortNumberBetween1And100);
                LogTelemetry("Attempt to add zero port");
                return null;
            }

            foreach (var apiConfig in CameraDataServices.Instance.CameraAPIConfigs.Configs)
            {
                Camera.APIConfig = apiConfig;
                try
                {
                    if (await Camera.GetCameraParams())
                    {
                        LogTelemetry("Added new camera.");

                        using (var stream = await Camera.GetSnapshotAsync())
                        {
                            if (stream != null)
                            {
                                Camera.SnapShotImageURI = await Storage.StoreAsync(stream, Locations.Roaming, String.Format("{0}.png", Camera.Id).Replace("-", ""), "SnapShots");
                                Camera.LastSnapshotUpdated = DateTime.Now;
                                return true;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

            }

            return false;
        }

        private async void SaveCamera()
        {
            await CameraDataServices.Instance.AddCamera(Camera);

            if (CameraDataServices.Instance.CameraSettings.IsSavingSettings.HasValue)
            {
                if (CameraDataServices.Instance.CameraSettings.IsSavingSettings.Value)
                {
                    CameraDataServices.Instance.CameraSettings.DefaultUserName = Camera.UserName;
                    CameraDataServices.Instance.CameraSettings.DefaultUrl = Camera.Url;
                    CameraDataServices.Instance.CameraSettings.DefaultPassword = Camera.Password;
                    CameraDataServices.Instance.CameraSettings.IsSavingSettings = true;
                    await CameraDataServices.Instance.SaveCameraSettings();
                }
            }
            else
            {
                if (await Popups.ConfirmAsync(CoolKidsCameraCommon.SaveDefaults, CoolKidsCameraCommon.SaveSettingsByDefault))
                {
                    CameraDataServices.Instance.CameraSettings.DefaultUserName = Camera.UserName;
                    CameraDataServices.Instance.CameraSettings.DefaultUrl = Camera.Url;
                    CameraDataServices.Instance.CameraSettings.DefaultPassword = Camera.Password;
                    CameraDataServices.Instance.CameraSettings.IsSavingSettings = true;
                    await CameraDataServices.Instance.SaveCameraSettings();
                }
                else
                {
                    CameraDataServices.Instance.CameraSettings.IsSavingSettings = true;
                    await CameraDataServices.Instance.SaveCameraSettings();
                }
            }

            LogTelemetry("Camera Successfully Added.",
            new KeyValuePair<string, string>("CameraMfg", String.IsNullOrEmpty(Camera.Manufacture) ? "UNKNOWN" : Camera.Manufacture),
            new KeyValuePair<string, string>("CameraModel", String.IsNullOrEmpty(Camera.Model) ? "UNKNOWN" : Camera.Model));

            GoBack();
        }

        private async void TestCameraSettings()
        {
            BusyMessage = CoolKidsCameraCommon.Connecting;
            IsBusy = true;
            var success = await TestCamera();
            IsBusy = false;

            if (success.HasValue)
            {
                if (success.Value)
                {
                    IsNextEnabled = true;
                    IsPrevEnabled = false;
                    NetworkSettingsVisible = false;
                    CameraPreviewVisible = true;
                    ScreenState = ScreenStates.CameraPreview;
                    ZoomInToPanel("PreviewGrid");
                }
                else
                {
                    LogTelemetry("Could not add new camera.");
                    await Popups.ShowAsync("NOPE NO LUCK!");
                }
            }
        }

        public async void Next()
        {
            switch (ScreenState)
            {
                case ScreenStates.NetworkSettings:
                    TestCameraSettings();
                    break;
                case ScreenStates.CameraPreview:
                    if (String.IsNullOrEmpty(Camera.CameraName))
                        await Popups.ShowAsync(CoolKidsCameraCommon.CameraNameIsRequired);
                    else
                    {
                        IsPrevEnabled = false;
                        IsNextVisible = false;
                        ScreenState = ScreenStates.AdvancedSettings;
                        AdvancedSettingsVisible = true;
                        CameraPreviewVisible = false;
                        ZoomInToPanel("AdvancedSettingsGrid");
                    }
                    break;
            }

        }

        public void Prev()
        {
            switch (ScreenState)
            {
                case ScreenStates.CameraPreview:
                    ScreenState = ScreenStates.NetworkSettings;
                    IsNextEnabled = false;
                    IsPrevVisible = false;
                    NetworkSettingsVisible = true;
                    CameraPreviewVisible = false;
                    ZoomOutToPanel("NetworkSettingsGrid");
                    break;
                case ScreenStates.AdvancedSettings:
                    ScreenState = ScreenStates.CameraPreview;
                    IsNextEnabled = false;
                    IsPrevEnabled = false;
                    IsSaveVisible = false;
                    AdvancedSettingsVisible = false;
                    CameraPreviewVisible = true;
                    ZoomOutToPanel("PreviewGrid");
                    break;
            }

        }

        public override void TransitionCompleted()
        {
            base.TransitionCompleted();

            switch (ScreenState)
            {
                case ScreenStates.NetworkSettings:
                    IsNextEnabled = true;
                    IsNextVisible = true;
                    IsPrevVisible = false;
                    break;

                case ScreenStates.CameraPreview:
                    IsNextVisible = true;
                    IsPrevVisible = true;
                    IsNextEnabled = true;
                    IsPrevEnabled = true;
                    break;

                case ScreenStates.AdvancedSettings:
                    IsPrevVisible = true;
                    IsPrevEnabled = true;
                    IsSaveVisible = true;
                    break;
            }
        }


        public void ShowTroubleShootingPage()
        {

        }

		CoolKids.Camera.Models.Camera _camera;
        public CoolKids.Camera.Models.Camera Camera
        {
            get { return _camera; }
            set { Set(ref _camera, value); }
        }

        private bool _networkSettingsVisible;
        public bool NetworkSettingsVisible
        {
            get { return _networkSettingsVisible; }
            set { Set(ref _networkSettingsVisible, value); }
        }


        private bool _cameraPreviewVisible;
        public bool CameraPreviewVisible
        {
            get { return _cameraPreviewVisible; }
            set { Set(ref _cameraPreviewVisible, value); }
        }

        private bool _advancedSettingsVisible;
        public bool AdvancedSettingsVisible
        {
            get { return _advancedSettingsVisible; }
            set { Set(ref _advancedSettingsVisible, value); }
        }


        private bool _finishVisible;
        public bool FinishVisible
        {
            get { return _finishVisible; }
            set { Set(ref _finishVisible, value); }
        }

        private bool _isNextVisible = false;

        public bool IsNextVisible
        {
            get { return _isNextVisible; }
            set { Set(ref _isNextVisible, value); }
        }

        private bool _isPrevVisible = false;

        public bool IsPrevVisible
        {
            get { return _isPrevVisible; }
            set { Set(ref _isPrevVisible, value); }
        }

        private bool _isNextEnabled = true;

        public bool IsNextEnabled
        {
            get { return _isNextEnabled; }
            set { Set(ref _isNextEnabled, value); }
        }

        private bool _isPrevEnabled = false;

        public bool IsPrevEnabled
        {
            get { return _isPrevEnabled; }
            set { Set(ref _isPrevEnabled, value); }
        }

        private bool _isSaveVisible = false;

        public bool IsSaveVisible
        {
            get { return _isSaveVisible; }
            set { Set(ref _isSaveVisible, value); }
        }

        public RelayCommand NextCommand { get { return new RelayCommand(() => Next()); } }
        public RelayCommand PrevCommand { get { return new RelayCommand(() => Prev()); } }
        public RelayCommand SaveCommand { get { return new RelayCommand(() => SaveCamera()); } }
        public RelayCommand ShowTroubleshootingPageCommand { get { return new RelayCommand(() => ShowTroubleShootingPage()); } }
        public RelayCommand TestSettingsCommand { get { return new RelayCommand(() => TestCameraSettings()); } }

    }
}
