﻿using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Models;
using OpenCvSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Interfaces
{
    public interface IYoloModelService
    {
        Task<List<YoloPrediction>> GetPredictions(Mat mat);

        WriteableBitmap PredictAndDraw(Mat image);

        WriteableBitmap PredictAndDraw(Camera camera, Mat mat, int scoreThreshold);
    }
}
