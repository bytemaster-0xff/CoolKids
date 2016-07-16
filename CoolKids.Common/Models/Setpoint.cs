using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Models
{
    public class Setpoint
    {
        public int Id { get; set; }
        public int ThermostatId { get; set; }
        public double HeatingSetpoint { get; set; }
        public double CoolingSetpoint { get; set; }
        public bool WeekDay { get; set; }
        public bool Weekend { get; set; }
        public int HourStart { get; set; }
        public int MinuteStart { get; set; }
        public bool IsEnabled { get; set; }
    }
}
