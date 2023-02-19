using System;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.Interfaces
{
    public interface ICustomPageService : IPageService
    {
        event EventHandler<string> OnPageNavigate;
    }
}
