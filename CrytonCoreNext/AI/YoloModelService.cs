using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.AI.Utils;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCoreNext.AI
{
    public class YoloModelService : IYoloModelService
    {
        private const string YoloModelONNXPath = "AI/YoloModels/yolov7-tiny.onnx";

        private const int MaxSizeOfDetectionQueue = 10;

        private YoloV7 _yolov7;

        public YoloModelService()
        {
            LoadYoloModel();
            LoadLabels();
        }

        public void LoadYoloModel(bool useCUDA = false)
        {
            _yolov7 = new YoloV7(YoloModelONNXPath, useCUDA);
        }

        public void LoadLabels(string pathToLabelsFile = "")
        {
            if (pathToLabelsFile == "")
            {
                _yolov7.SetupYoloDefaultLabels();
            }
        }

        public Mat PredictAndDraw(Mat mat)
        {
            using var image = mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var predicitons = _yolov7.Predict(image);
            return YoloDetectionDrawer.DrawPredicitons(mat, predicitons);
        }

        public async Task<List<YoloPrediction>> GetPredictions(Mat mat)
        {
            switch (mat.Channels())
            {
                case 4:
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.RGBA2RGB);
                    break;
                case 1:
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.GRAY2BGR);
                    break;
            }
            using var image = mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return await Task.Run(() => { return _yolov7.Predict(image); });
        }

        public Mat PredictAndDraw(Camera camera, Mat mat, int scoreThreshold)
        {
            Draw(camera, mat, _yolov7.Predict(mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb)), scoreThreshold);
            return mat;
        }

        private static void Draw(Camera camera, Mat mat, List<YoloPrediction> predictions, int scoreThreshold)
        {
            if (predictions != null)
            {
                foreach (var prediction in predictions)
                {
                    if (prediction.Label == null)
                    {
                        continue;
                    }
                    if (camera.CameraDetectionsQueue.Count() > MaxSizeOfDetectionQueue)
                    {
                        _ = camera.CameraDetectionsQueue.Dequeue();
                    }
                    var score = prediction.Score * 100;
                    if (score >= scoreThreshold)
                    {
                        camera.CameraDetectionsQueue.Enqueue(new CameraDetection(prediction.Label?.Name?.ToString(), score.ToString("N1")));
                    }
                    var color = new Scalar(
                        prediction.Label.Color.R,
                        prediction.Label.Color.G,
                        prediction.Label.Color.B);
                    var rect = new Rect(
                        (int)prediction.Rectangle.X,
                        (int)prediction.Rectangle.Y,
                        (int)prediction.Rectangle.Width,
                        (int)prediction.Rectangle.Height);
                    Cv2.Rectangle(mat, rect, color);
                    Cv2.PutText(mat, prediction.Label.Name, new Point(
                        prediction.Rectangle.X - 7,
                        prediction.Rectangle.Y - 23), HersheyFonts.HersheyPlain, 1, color, 2);
                }
            }
        }
    }
}
