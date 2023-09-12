using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingViewModel
    {
        string ExportObjects();

        bool ImportObjects(string str);
    }
}
