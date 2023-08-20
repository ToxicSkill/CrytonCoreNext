using CrytonCoreNext.AI.Models;
using OpenCvSharp;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Interfaces
{
    public interface IYoloModelService
    {
        bool LoadYoloModel(string path, bool useCUDA = false);

        void LoadLabels(string pathToLabelsFile = "");

        List<YoloPrediction> GetPredictions(Mat mat);

        WriteableBitmap PredictAndDraw(Mat image);
    }
}
