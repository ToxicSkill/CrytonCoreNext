using CrytonCoreNext.Models;

namespace CrytonCoreNext.AI.Models
{
    public class AIDetectionImage : SimpleImageItemContainer
    {
        public YoloPrediction Prediction { get; init; }

        public AIImage Parent { get; }

        public AIDetectionImage(AIImage parent, YoloPrediction yoloPrediction)
        {
            Prediction = yoloPrediction;
            Parent = parent;
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
