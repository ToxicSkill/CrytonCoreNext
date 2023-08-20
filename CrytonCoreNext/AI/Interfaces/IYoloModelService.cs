using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Interfaces
{
    public interface IYoloModelService
    {
        bool LoadYoloModel(string path, bool useCUDA = false);

        void LoadLabels(string pathToLabelsFile = "");

        WriteableBitmap PredictAndDraw(Mat image);
    }
}
