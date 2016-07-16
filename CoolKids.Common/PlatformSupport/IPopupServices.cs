using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.PlatformSupport
{
    public interface IPopupServices
    {
        Task<bool> ConfirmAsync(String title, String prompt);

        Task ShowAsync(String title, String message);
        Task ShowAsync(String message);
    }
}
