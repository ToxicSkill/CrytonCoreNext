﻿using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Models;
using Nito.AsyncEx;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CrytonCoreNext.AI.Models
{
    public partial class ImageParameter : ObservableObject
    {
        private Action _valueChangedAction;

        [ObservableProperty]
        private string name;

        public double Value { get; private set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public double DefaultValue { get; init; }

        [ObservableProperty]
        private double rangeValue;

        public ImageParameter(string name, double minValue, double maxValue, double defaultValue)
        {
            this.name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            DefaultValue = defaultValue;
            SanityCheck();
            SetDefault();
        }

        private void SanityCheck()
        {
            if (MaxValue < MinValue)
            {
                throw new ArgumentException("MinValue can not be greater than MaxValue");
            }
            if (DefaultValue > MaxValue || DefaultValue < MinValue)
            {
                throw new ArgumentException("DefaultValue is not in given min-max range");
            }
        }

        partial void OnRangeValueChanged(double value)
        {
            Value = value.ConvertForRange(0, 100, MinValue, MaxValue);
            _valueChangedAction?.Invoke();
        }

        public void RegisterValueUpdateAction(Action action)
        {
            _valueChangedAction = action;
        }

        public void SetDefault()
        {
            RangeValue = DefaultValue.ConvertForRange(MinValue, MaxValue, 0, 100);
        }
    }

    public partial class AIImage : SimpleImageItemContainer
    {
        public static double DefaultAutoColorValue = 0.5;

        private readonly ImageDrawer _drawer;

        private const int HighQualityImageRenderDelayInSeconds = 2;

        private readonly List<Task> _tasks = [];

        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private ImageParameter contrast = new(nameof(Contrast), 0, 2, 1);

        [ObservableProperty]
        private ImageParameter autoColor = new(nameof(AutoColor), 0, 2, 0.5);

        [ObservableProperty]
        private ImageParameter exposure = new(nameof(Exposure), 0, 2, 1);

        [ObservableProperty]
        private ImageParameter brightness = new(nameof(Brightness), -127, 127, 0);

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        public Mat ResizedImage { get; set; }

        public bool RenderFinal { get; set; }

        [ObservableProperty]
        public string detectionLabel;

        [ObservableProperty]
        public System.Drawing.Size constrains;

        [ObservableProperty]
        public bool normalizeRGBHistogram;

        [ObservableProperty]
        public bool normalizeLABHistogram;

        [ObservableProperty]
        public bool drawGrayscale;

        [ObservableProperty]
        public WriteableBitmap histogram;

        [ObservableProperty]
        public WriteableBitmap detectionImage;

        [ObservableProperty]
        public WriteableBitmap adjusterImage;

        public AIImage(string path, ImageDrawer drawer)
        {
            _timer = new DispatcherTimer(TimeSpan.FromSeconds(HighQualityImageRenderDelayInSeconds), DispatcherPriority.Render, (s, e) => RenderHighQuality(), App.Current?.Dispatcher);
            _drawer = drawer;
            Path = path;
            Label = System.IO.Path.GetFileName(path);
            DetectionImages = [];
            Predictions = [];

            Exposure.RegisterValueUpdateAction(UpdateImage);
            Brightness.RegisterValueUpdateAction(UpdateImage);
            Contrast.RegisterValueUpdateAction(UpdateImage);
        }

        private void RenderHighQuality()
        {
            RenderFinal = true;
            UpdateImage();
        }

        public void UpdateImage()
        {
            AbortHighQualityRender();
            _tasks.Add(Task.Run(() => _drawer.Post(this)));
            _tasks.WhenAll();
            _timer.Start();
        }

        public void AbortHighQualityRender()
        {
            _timer.Stop();
        }

        public bool IsImageReady()
        {
            return _drawer.IsReady;
        }


        partial void OnNormalizeLABHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        partial void OnNormalizeRGBHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        partial void OnDrawGrayscaleChanged(bool value)
        {
            UpdateImage();
        }

        public void SetPredicitons(List<YoloPrediction> predictions, Mat mat)
        {
            var random = new Random();
            Predictions.Clear();
            Predictions.AddRange(predictions);
            foreach (var prediction in Predictions)
            {
                if (prediction == null || prediction.Label == null)
                {
                    continue;
                }
                prediction.Label.Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                ExtractDetectionImagesFromBitmap(prediction, mat);
            }
        }

        private void ExtractDetectionImagesFromBitmap(YoloPrediction prediction, Mat mat)
        {
            if (prediction == null) return;
            var rectangle = prediction.Rectangle.ToRect();
            var newX = Math.Clamp(rectangle.X, 0, mat.Width);
            var newY = Math.Clamp(rectangle.Y, 0, mat.Height);
            var newWidth = Math.Clamp(rectangle.Width, 0, mat.Width - newX);
            var newHeight = Math.Clamp(rectangle.Height, 0, mat.Height - newY);
            var newRect = new Rect(newX, newY, newWidth, newHeight);
            prediction.Rectangle = new RectangleF(newRect.X, newRect.Y, newRect.Width, newRect.Height);
            DetectionImages.Add(
                new(this, prediction)
                {
                    Image = new Mat(mat, newRect).ToWriteableBitmap()
                });
        }
    }
}
