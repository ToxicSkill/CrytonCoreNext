using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        private readonly ImageDrawer _drawer;

        private const int MaxSingleDimensionSize = 1024;

        private const int HighQualityImageRenderDelayInSeconds = 2;

        private readonly List<Task> _tasks = [];

        private readonly DispatcherFrame _frame;

        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private ImageParameter contrast = new(nameof(Contrast), 0, 2, 1);

        [ObservableProperty]
        private ImageParameter autoColor = new(nameof(AutoColor), 0, 2, 0.5);

        [ObservableProperty]
        private ImageParameter exposure = new(nameof(Exposure), 0, 2, 1);

        [ObservableProperty]
        private ImageParameter brightness = new(nameof(Brightness), -127, 127, 0);

        public static double DefaultAutoColorValue = 0.5;

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        public List<Path> Paths { get; set; }

        public Mat PipelineMat { get; set; }

        public Mat ResizedImage { get; set; }

        public Bitmap HistogramBitmap { get; set; }

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

            DetectionImages = [];
            Predictions = [];

            Exposure.RegisterValueUpdateAction(UpdateImage);
            Brightness.RegisterValueUpdateAction(UpdateImage);
            Contrast.RegisterValueUpdateAction(UpdateImage);

            LoadImages();
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

        private void LoadImages()
        {
            using var image = Cv2.ImRead(Path, ImreadModes.Unchanged);
            if (image.Empty())
            {
                return;
            }
            Image = image.ToWriteableBitmap();
            Label = System.IO.Path.GetFileName(Path);
            Constrains = new System.Drawing.Size((int)Image.Width, (int)Image.Height);
            DetectionImage = Image;
            AdjusterImage = Image;
            var constrains = new List<double>() { Image.Width, Image.Height };
            if (constrains.Any(x => x > MaxSingleDimensionSize))
            {
                var max = constrains.Max();
                var ratio = MaxSingleDimensionSize / max;
                var newHeight = Image.Height;
                var newWidth = Image.Width;
                if (Image.Width > Image.Height)
                {
                    newHeight *= ratio;
                    newWidth = MaxSingleDimensionSize;
                }
                else
                {
                    newWidth *= ratio;
                    newHeight = MaxSingleDimensionSize;
                }
                ResizedImage = Image.ToMat().EmptyClone();
                Cv2.Resize(Image.ToMat(), ResizedImage, new OpenCvSharp.Size(newWidth, newHeight));
            }
            else
            {
                ResizedImage = Image.ToMat();
            }
            UpdateImage();
        }


        partial void OnNormalizeLABHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        partial void OnNormalizeRGBHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        public void SetPredicitons(List<YoloPrediction> predictions)
        {
            foreach (var prediction in Predictions)
            {
                var random = new Random();
                prediction.Label.Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            }
            Predictions = predictions;
            ExtractDetectionImagesFromBitmap();
        }

        private void ExtractDetectionImagesFromBitmap()
        {
            using var mat = Image.ToMat();
            foreach (var prediction in Predictions)
            {
                var rectangle = prediction.Rectangle.ToRect();
                var newWidth = Math.Clamp(rectangle.Width, 0, mat.Width - rectangle.X);
                var newHeight = Math.Clamp(rectangle.Height, 0, mat.Height - rectangle.Y);
                var newX = Math.Clamp(rectangle.X, 0, mat.Width);
                var newY = Math.Clamp(rectangle.Y, 0, mat.Height);
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
}
