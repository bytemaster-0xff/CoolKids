using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Camera.Models
{
    public interface ICameraStream
    {
        void ParseStream(Uri uri, String userName, String lastName);
        void StopStream();
    }
}
