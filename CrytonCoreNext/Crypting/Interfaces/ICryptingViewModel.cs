namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingViewModel
    {
        byte[] ExportObjects();

        bool ImportObjects(string str);
    }
}
