using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation.Models
{
    public class Attribute
    {
        public string label { get; set; }
        public string protection { get; set; }
        public string dataType { get; set; }
        public string value { get; set; }
        public string defaultValue { get; set; }
        public object lastUpdateTime { get; set; }
    }

    public class DL_Device_Content
    {
        public string deviceType { get; set; }
        public string deviceGuid { get; set; }
        public bool movable { get; set; }
        public string supportUrl { get; set; }
        public List<object> events { get; set; }
        public List<Attribute> attributes { get; set; }
    }


    public class DL_Device
    {
        public string status { get; set; }
        public List<DL_Device_Content> content { get; set; }
    }
    
}
