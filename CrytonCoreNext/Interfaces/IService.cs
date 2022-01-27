using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface IService
    {
        bool GetStatus();

        Task InitializeService(object obj);
    }
}
