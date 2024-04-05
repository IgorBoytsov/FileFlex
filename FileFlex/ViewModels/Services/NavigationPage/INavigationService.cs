using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFlex.ViewModels.Services
{
    interface INavigationService
    {
        void NavigateTo(string pageName, object parameter);
    }
}
