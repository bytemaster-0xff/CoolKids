using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
        public String Type { get; set; }
        public String IRCode { get; set; }
    }
}
