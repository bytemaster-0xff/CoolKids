using CoolKids.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public interface ISSDPFinder
    {
        event EventHandler<uPnPDevice> NewDeviceFound;

        Task SsdpQueryAsync(string filter = "ssdp:all", int seconds = 5);

        void Cancel();

        bool ShowDiagnostics { get; set; }
    }
}
