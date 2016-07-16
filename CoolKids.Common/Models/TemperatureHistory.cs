using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Models
{
    [DataContract(Name="TemperatureHistory")]
    public class TemperatureHistory
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ThermostatId { get; set; }
        [DataMember]
        public bool Heating { get; set; }
        [DataMember]
        public bool Cooling { get; set; }
        [DataMember]
        public double Temperature { get; set; }
        [DataMember]
        public double Humidity { get; set; }
        [DataMember]
        public DateTime DateStamp { get; set; }
        [DataMember]
        public bool Online { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public double HeatingSetpoint { get; set; }
        [DataMember]
        public double CoolingSetpoint { get; set; }

        [IgnoreDataMember]
        public DateTime DateStampLocal { get { return DateStamp.ToLocalTime(); } }

        [IgnoreDataMember] public String TemperatureDisplay { get { return String.Format("{0}° F", Temperature); } }
        [IgnoreDataMember] public String HumidityDisplay { get { return String.Format("{0}%", Humidity); } }
     
    }
}
