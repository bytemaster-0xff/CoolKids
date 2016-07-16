using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common
{
    public interface IConsoleWriter
    {
        void WriteLine(String message);
        void WriteError(String message);
        void WriteWarning(String message);
    }

    public interface ILocalTime
    {
        DateTime Now { get; }
    }
}
