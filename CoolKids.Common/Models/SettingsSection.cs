using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Models
{
    public class SettingsSection
    {
        public String Name { get; set; }
        public String Key { get; set; }
        public bool IsVisible { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
