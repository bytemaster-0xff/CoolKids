using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public interface ITileServices
    {
        Task Pin(String tileId, String shortName, string launchArguments, Uri imageUri);
    }
}
