using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic; 

namespace CrytonCoreNext.AI.Models
{
    public class AIImage : SimpleImageItemContainer
    {
        public List<YoloPrediction> Predictions { get; private set; }

        public AIImage(string path)
        {
            Image = Cv2.ImRead(path).ToWriteableBitmap();
            Predictions = new List<YoloPrediction>();
            Label = System.IO.Path.GetFileName(path);
        }

        public void SetPredicitons(List<YoloPrediction> predictions)
        {
            Predictions = predictions;
        }
    }
}
