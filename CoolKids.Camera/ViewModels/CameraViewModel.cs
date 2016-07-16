using CoolKids.Common.Commanding;
using CoolKids.Camera.Models;
using CoolKids.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.ViewModels
{
    public class CameraViewModel : ViewModelBase
    {
        public override Task InitAsync()
        {
           IPCamera = NavigationParameter as CoolKids.Camera.Models.Camera; 

            return base.InitAsync();
        }

        public override void DidClose()
        {
            IPCamera = null;
        }

        private CoolKids.Camera.Models.Camera _ipCamera = new CoolKids.Camera.Models.Camera();
        public CoolKids.Camera.Models.Camera IPCamera
        {
            get { return _ipCamera; }
            set { Set(ref _ipCamera, value); }
        }

        public async void DeleteCamera()
        {
            if(await Popups.ConfirmAsync(CoolKidsCameraCommon.AreYouSure, CoolKidsCameraCommon.ApplicationTitle))
            {
                await CameraDataServices.Instance.DeleteCamera(IPCamera);

                GoBack();
            }
        }

        public RelayCommand DeleteCameraCommand { get { return new RelayCommand(() => DeleteCamera()); } }
    }
}
