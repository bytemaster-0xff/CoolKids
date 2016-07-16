using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Utils
{
    public interface IWatchdog
    {
        event EventHandler Elapsed;

        TimeSpan Period { get; set; }
        void Enable();
        void Disable();
        void Feed();
    }
}
