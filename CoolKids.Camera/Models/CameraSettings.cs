using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.Models
{
    public class CameraSettings
    {
        public String DefaultUrl { get; set; }
        public bool? IsSavingSettings { get; set; }
        public bool ViewPassword { get; set; }

        public String DefaultUserName { get; set; }
        public String DefaultPassword { get; set; }
        public bool FlipLiveTilesOnStartScreen { get; set; }
        public bool FlipLiveTilesInApp { get; set; }
        public String UpdateLockScreen { get; set; }
        public Boolean UpdateTilesInBackground { get; set; }
        public Boolean HasRatedApp { get; set; }
    
    }
}
