using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{

    public class NetworkDetails
    {
        public bool IsWireless { get; set; }
        public String SSID { get; set; }
        public String Name { get; set; }
        public String IPAddress { get; set; }
        public String Gateway { get; set; }
        public String Connectivity { get; set; }
    }

    public interface INetworkService
    {
        event EventHandler NetworkInformationChanged;

        bool IsInternetConnected { get; }

        string GetIPV4Address();

        Task RefreshAysnc();
        
        ObservableCollection<NetworkDetails> AllConnections { get; }

        Task<bool> TestConnectivityAsync();
    }
}
