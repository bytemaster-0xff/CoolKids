using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Utils
{
    public interface IUIThread
    {
        void Invoke(Action action);
    }
}
