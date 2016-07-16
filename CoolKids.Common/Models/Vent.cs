using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Models
{
    [DataContract(Name="Vents")]
    public class Vent
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public int RoomId { get; set; }
        [DataMember] public String Name { get; set; }
        [DataMember] public String State { get; set; }
        [DataMember] public String ImpID { get; set; }
    }
}
