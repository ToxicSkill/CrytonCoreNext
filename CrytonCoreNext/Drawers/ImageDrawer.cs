﻿using CrytonCoreNext.AI.Models;
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
    public struct Context
    {
        public bool DrawLABHistogram { get; init; }
        public bool DrawRGBHistogram { get; init; }
        public double Brightness { get; init; }
        public double Exposure { get; init; }
        public double Contrast { get; init; }
        public Mat Image { get; set; }
        public Bitmap Histogram { get; set; }
        public AIImage AiImage { get; init; }
        public Context(AIImage image)
        {
            DrawLABHistogram = image.NormalizeLABHistogram;
            DrawRGBHistogram = image.NormalizeRGBHistogram;
            Brightness = image.BrightnessValue;
            Exposure = image.ExposureValue;
            Contrast = image.ContrastValue;
            Image = image.ResizedImage.Clone();
            AiImage = image;
        }
    }

    public class ImageDrawer
    {
        private const System.Windows.Threading.DispatcherPriority DispatcherPriority = System.Windows.Threading.DispatcherPriority.Background;

        private readonly TransformBlock<AIImage, Context> _pipeline;

        private readonly SemaphoreSlim _semaphore;

        public ImageDrawer()
        {
            _semaphore = new SemaphoreSlim(1);
            _pipeline = CreatePipeline(Finalize);
        }

        public async Task Post(AIImage image)
        {
            try
            {
                await _semaphore.WaitAsync();
                await _pipeline.SendAsync(image);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
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
            var outputBlock = new ActionBlock<Context>(result);
            inputBlock.LinkTo(step2, dfLinkOptions);
            step2.LinkTo(step3, dfLinkOptions);
            step3.LinkTo(step4, dfLinkOptions);
            step4.LinkTo(step5, dfLinkOptions);
            step5.LinkTo(step6, dfLinkOptions);
            step6.LinkTo(outputBlock, dfLinkOptions);
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

        private static Context NormalizeLABHistogram(Context context)
        {
            if (!context.DrawLABHistogram)
            {
                return context;
            }
            var labColorMat = new Mat();
            Cv2.CvtColor(context.Image, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, new(8, 8)).Apply(channels[0], channels[0]);
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
            for (var i = 0; i < 3; i++)
            {
                Cv2.CreateCLAHE(2.0, new(8, 8)).Apply(channels[i], channels[i]);
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
