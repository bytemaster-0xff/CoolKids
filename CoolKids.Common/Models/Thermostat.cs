using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Models
{
    public class Thermostat
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public String DisplayName { get; set; }
        public String XivelyFeedName { get; set; }
        public String InsteonID { get; set; }
        public Double Temperature { get; set; }
        public Double Humidity { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsHeating { get; set; }
        public bool IsCooling { get; set; }
        public bool IsEnabled { get; set; }
        public bool HeatingEnabled { get; set; }
        public bool CoolingEnabled { get; set; }
        public double CoolingSetpoint { get; set; }
        public double HeatingSetpoint { get; set; }
        public int DamperOpenRelay { get; set; }
        public int DamperCloseRelay { get; set; }
        public int VentOpenRelay { get; set; }
        public int VentCloseRelay { get; set; }
    }
}
