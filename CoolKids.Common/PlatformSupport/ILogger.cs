using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public enum LogLevel
    {
        Error,
        Warning,
        Message,
        Verbose,
    }

    public interface ILogger
    {
        void SetKeys(params string[] args);

        void Log(LogLevel level, String area, String message, params KeyValuePair<String, String>[] args);

        void LogException(String area, Exception ex, params KeyValuePair<String, String>[] args);
    }
}
