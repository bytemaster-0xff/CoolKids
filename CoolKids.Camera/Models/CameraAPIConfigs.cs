using CoolKids.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.Models
{
    public class CameraAPIConfigs
    {
        [DataMember]
        public double Version { get; set; }

        [DataMember]
        public DateTime DateStamp { get; set; }

        [DataMember]
        public List<CameraAPIConfig> Configs { get; set; }
    }
}
