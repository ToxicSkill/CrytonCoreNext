﻿using System.Drawing;

namespace CrytonCoreNext.AI.Models
{
    public class YoloPrediction
    {
        public YoloLabel? Label { get; set; }

        public RectangleF Rectangle { get; set; }

        public float Score { get; set; }

        public YoloPrediction(YoloLabel label, float confidence)
            : this(label)
        {
            Score = confidence;
        }

        public YoloPrediction(YoloLabel label)
        {
            Label = label;
        }
    }
}
    