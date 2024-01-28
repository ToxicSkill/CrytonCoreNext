using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Models;
using OpenCvSharp;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Interfaces
{
    public interface IYoloModelService
    {
        private const string YoloModelONNXPath = "AI/YoloModels/yolov7-tiny.onnx";

        bool LoadYoloModel(string path = YoloModelONNXPath, bool useCUDA = false);

        void LoadLabels(string pathToLabelsFile = "");

        List<YoloPrediction> GetPredictions(Mat mat);

        WriteableBitmap PredictAndDraw(Mat image);

        WriteableBitmap PredictAndDraw(Camera camera, Mat mat, int scoreThreshold);
    }
}
