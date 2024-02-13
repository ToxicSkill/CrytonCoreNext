using System;
using Wpf.Ui;


namespace CrytonCoreNext.Interfaces
{
    public interface ICustomPageService : IPageService
    {
        event EventHandler<string> OnPageNavigate;
    }
}
