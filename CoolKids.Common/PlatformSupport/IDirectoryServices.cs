using CoolKids.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public interface IDirectoryServices
    {
        List<Folder> GetFolders(String parent);
        List<File> GetFiles(String directory);
    }
}
