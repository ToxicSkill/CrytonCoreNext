using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Native;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace CrytonCoreNext.Drawers
{
    public struct Context(AIImage image)
    {
        public bool DrawLABHistogram { get; init; } = image.NormalizeLABHistogram;
        public bool DrawRGBHistogram { get; init; } = image.NormalizeRGBHistogram;
        public bool DrawGrayscale { get; init; } = image.DrawGrayscale;
        public double Brightness { get; init; } = image.Brightness.Value;
        public double Exposure { get; init; } = image.Exposure.Value;
        public double Contrast { get; init; } = image.Contrast.Value;
        public Mat Image { get; set; } = image.RenderFinal ? Application.Current?.Dispatcher.Invoke(image.Image.ToMat) : image.ResizedImage.Clone();
        public Bitmap Histogram { get; set; }
        public AIImage AiImage { get; init; } = image;
    }

    public class ImageDrawer
    {
        private const System.Windows.Threading.DispatcherPriority DispatcherPriority = System.Windows.Threading.DispatcherPriority.Background;

        private static readonly OpenCvSharp.Size? CLAHEKernelSize = new(8, 8);

        private readonly TransformBlock<AIImage, Context> _pipeline;

        private readonly SemaphoreSlim _semaphore;

        private int _threadSafeBoolBackValue = 0;

        public bool IsReady
        {
            get { return (Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 1) == 1); }
            set
            {
                if (value) Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 0, 1);
            }
        }

        public ImageDrawer()
        {
            _semaphore = new SemaphoreSlim(1);
            _pipeline = CreatePipeline(Finalize);
        }

        public async Task Post(AIImage image)
        {
            try
            {
                IsReady = false;
                await _semaphore.WaitAsync();
                await _pipeline.SendAsync(image);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsReady = true;
                _semaphore.Release();
            }
        }

        public static TransformBlock<AIImage, Context> CreatePipeline(Action<Context> result)
        {
            var dfBlockOptions = new ExecutionDataflowBlockOptions();
            var dfLinkOptions = new DataflowLinkOptions()
            {
                PropagateCompletion = false
            };
            var inputBlock = new TransformBlock<AIImage, Context>(CopyOriginalImage, dfBlockOptions);
            var step2 = new TransformBlock<Context, Context>(NormalizeLABHistogram, dfBlockOptions);
            var step3 = new TransformBlock<Context, Context>(NormalizeRGBHistogram, dfBlockOptions);
            var step4 = new TransformBlock<Context, Context>(SetBrightness, dfBlockOptions);
            var step5 = new TransformBlock<Context, Context>(SetExposure, dfBlockOptions);
            var step6 = new TransformBlock<Context, Context>(DrawHistogram, dfBlockOptions);
            var step7 = new TransformBlock<Context, Context>(DrawGrayscale, dfBlockOptions);
            var outputBlock = new ActionBlock<Context>(result);
            inputBlock.LinkTo(step2, dfLinkOptions);
            step2.LinkTo(step3, dfLinkOptions);
            step3.LinkTo(step4, dfLinkOptions);
            step4.LinkTo(step5, dfLinkOptions);
            step5.LinkTo(step6, dfLinkOptions);
            step6.LinkTo(step7, dfLinkOptions);
            step7.LinkTo(outputBlock, dfLinkOptions);
            return inputBlock;
        }

        private static async void Finalize(Context context)
        {
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                context.AiImage.Histogram ??= context.Histogram.ToMat().ToWriteableBitmap();
                var width = context.Histogram.Width;
                var height = context.Histogram.Height;
                BitmapData data = context.Histogram.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, context.Histogram.PixelFormat);
                context.AiImage.Histogram.Lock();
                NativeMethods.CopyMemory(context.AiImage.Histogram.BackBuffer, data.Scan0, data.Stride * height);
                context.AiImage.Histogram.AddDirtyRect(new Int32Rect(0, 0, width, height));
                context.AiImage.Histogram.Unlock();
                context.Histogram.UnlockBits(data);
                context.Histogram.Dispose();
                context.AiImage.AdjusterImage = context.Image.ToWriteableBitmap();
                context.AiImage.RenderFinal = false;
            }, DispatcherPriority);
        }

        private static Context CopyOriginalImage(AIImage image)
        {
            return new Context(image);
        }

        private static Context DrawHistogram(Context context)
        {
            context.Histogram = HistogramDrawer.CalcualteHistogram(context.Image);
            return context;
        }

        private static Context DrawGrayscale(Context context)
        {
            if (!context.DrawGrayscale)
            {
                return context;
            }
            switch (context.Image.Channels())
            {
                case 3:
                    Cv2.CvtColor(context.Image, context.Image, ColorConversionCodes.BGR2GRAY);
                    break;
                case 4:
                    Cv2.CvtColor(context.Image, context.Image, ColorConversionCodes.BGRA2GRAY);
                    break;
                default:
                    break;
            }
            return context;
        }

        private static Context NormalizeLABHistogram(Context context)
        {
            if (!context.DrawLABHistogram)
            {
                return context;
            }
            var labColorMat = new Mat();
            Cv2.CvtColor(context.Image, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, CLAHEKernelSize).Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(context.Image, 1 - AIImage.DefaultAutoColorValue, labColorMat, AIImage.DefaultAutoColorValue, 0, context.Image);
            return context;
        }

        private static Context NormalizeRGBHistogram(Context context)
        {
            if (!context.DrawRGBHistogram)
            {
                return context;
            }
            using var rgbColorMat = context.Image.Clone();
            var channels = Cv2.Split(rgbColorMat);
            for (var i = 0; i < channels.Length; i++)
            {
                Cv2.CreateCLAHE(2.0, CLAHEKernelSize).Apply(channels[i], channels[i]);
            }
            Cv2.Merge(channels, rgbColorMat);
            Cv2.AddWeighted(context.Image, 1 - AIImage.DefaultAutoColorValue, rgbColorMat, AIImage.DefaultAutoColorValue, 0, context.Image);
            foreach (var channel in channels)
            {
                channel.Dispose();
            }
            return context;
        }

        private static Context SetBrightness(Context context)
        {
            Cv2.ConvertScaleAbs(context.Image, context.Image, context.Contrast, context.Brightness);
            return context;
        }

        private static Context SetExposure(Context context)
        {
            var expMat = new Mat();
            Cv2.CvtColor(context.Image, expMat, ColorConversionCodes.BGR2HSV);
            var channels = Cv2.Split(expMat);
            Cv2.ConvertScaleAbs(channels[2], channels[2], context.Exposure);
            Cv2.Merge(channels, expMat);
            Cv2.CvtColor(expMat, context.Image, ColorConversionCodes.HSV2BGR);
            foreach (var channel in channels)
            {
                channel.Dispose();
            }
            return context;
        }
    }
}
