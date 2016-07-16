using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation.Models
{
    public class DL_DeviceAttribute_Content
    { 
        public string label { get; set; }
        public string protection { get; set; }
        public string dataType { get; set; }
        public string value { get; set; }
        public string defaultValue { get; set; }
        public long lastUpdateTime { get; set; }
    }


    public class DL_DeviceAttribute
    {
        public string status { get; set; }
        public DL_DeviceAttribute_Content content { get; set; }
    }
}
