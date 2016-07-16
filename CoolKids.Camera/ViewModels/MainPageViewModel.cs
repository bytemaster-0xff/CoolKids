using CoolKids.Common.Commanding;
using CoolKids.Camera.Models;
using CoolKids.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolKids.Common;

namespace CoolKids.Camera.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public async override Task InitAsync()
        {
            IsBusy = true;

            await CameraDataServices.Instance.InitAsync();
            Cameras = CameraDataServices.Instance.Cameras.ToObservableCollection();
            IsBusy = false;
        }

        private ObservableCollection<CoolKids.Camera.Models.Camera> _cameras;
        public ObservableCollection<CoolKids.Camera.Models.Camera> Cameras
        {
            get { return _cameras; }
            set { Set(ref _cameras, value); }
        }

        private CoolKids.Camera.Models.Camera _camera;
        public CoolKids.Camera.Models.Camera Camera
        {
            get { return _camera; }
            set
            {
                var previousCamera = _camera;
                Set(ref _camera, value);
                if (previousCamera != value && value != null)
                    ShowViewModel<CameraViewModel>(_camera);
            }
        }

        public async void RefreshCameras()
        {
            await PerformNetworkOperation(async () =>
            {
                await CameraDataServices.Instance.RefreshCameraSnapshotsAsync();
            });
        }

        public void AddCamera()
        {
            ShowViewModel<ViewModels.AddCameraViewModel>();
        }

        public RelayCommand AddCameraCommand { get { return new RelayCommand(() => AddCamera()); } }

        public RelayCommand RefreshCamerasCommand { get { return new RelayCommand(() => RefreshCameras()); } }

    }
}
