using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Resources
{
    public class IPCameraStrings
    {
		private static IPCameraResources _localizedResources = new IPCameraResources();

		public static IPCameraResources LocalizedResources { get { return _localizedResources; } }
	}
}