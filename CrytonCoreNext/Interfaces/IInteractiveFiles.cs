using System.Windows.Media;

namespace CrytonCoreNext.Interfaces
{
    public interface IInteractiveFiles
    {
        void ClearAllFiles();

        void AddFiles();

        void PostPopup(string informationString, int seconds, Color color = default);

        void DeleteItem();
    }
}
