using CrytonCoreNext.Models;

namespace CrytonCoreNext.AI.Models
{
    public class AIDetectionImage : SimpleImageItemContainer
    {
        public YoloPrediction Prediction { get; init; }

        public AIDetectionImage(YoloPrediction yoloPrediction)
        {
            Prediction = yoloPrediction;
            if (Prediction != null)
            {
                if (Prediction.Label != null)
                {
                    Label = Prediction.Label.Name ?? "N/A";
                }
            }
        }
    }
}
