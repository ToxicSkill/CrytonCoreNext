using CrytonCoreNext.Models;

namespace CrytonCoreNext.Interfaces
{
    public interface IEmailService
    {
        void SendMail(EMail email);
    }
}