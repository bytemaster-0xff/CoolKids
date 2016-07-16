using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.Models
{
    public class CameraAPIConfig
    {
        public String ProfileName { get; set; }

        public String Description { get; set; }

        public int APIIndex;
        public String VideoURI {get; set;}

        public Boolean SupportsSnapShot { get; set; }

        public String SnapshotURI { get; set; }

        public String SendSettingURI { get; set; }

        public String SendCameraAction { get; set; }

        public String GetCameraSettings1 { get; set; }

        public String GetCameraSettings2 { get; set; }

        public String GetCameraSettings3 { get; set; }

        public bool AreParamsXML { get; set; }
        public bool IsDigestAuth { get; set; }
    }
}
