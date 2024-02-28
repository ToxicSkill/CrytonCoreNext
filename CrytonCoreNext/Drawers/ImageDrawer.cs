﻿using CrytonCoreNext.AI.Models;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCoreNext.Drawers
{
    public class ImageDrawer
    {
        private const System.Windows.Threading.DispatcherPriority DispatcherPriority = System.Windows.Threading.DispatcherPriority.Background;

        private readonly TransformBlock<AIImage, AIImage> _pipeline;

        private readonly SemaphoreSlim _semaphore;

        public ImageDrawer()
        {
            _semaphore = new SemaphoreSlim(1);
            _pipeline = CreatePipeline(UpdateOutput);
        }

        private void Release()
        {
            _semaphore.Release();
        }

        public async Task Post(AIImage image, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                _pipeline.Complete();
            }
            try
            {
                await _semaphore.WaitAsync(token);
                if (!token.IsCancellationRequested)
                {
                    await _pipeline.SendAsync(image);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public static TransformBlock<AIImage, AIImage> CreatePipeline(Action<AIImage> result)
        {
            var dfBlockOptions = new ExecutionDataflowBlockOptions();
            var dfLinkOptions = new DataflowLinkOptions()
            {
                PropagateCompletion = true
            };
            var inputBlock = new TransformBlock<AIImage, AIImage>(CopyOriginalImage, dfBlockOptions);
            var step2 = new TransformBlock<AIImage, AIImage>(NormalizeLABHistogram, dfBlockOptions);
            var step3 = new TransformBlock<AIImage, AIImage>(NormalizeRGBHistogram, dfBlockOptions);
            var step4 = new TransformBlock<AIImage, AIImage>(SetBrightness, dfBlockOptions);
            var step5 = new TransformBlock<AIImage, AIImage>(SetExposure, dfBlockOptions);
            var step6 = new TransformBlock<AIImage, AIImage>(DrawHistogram, dfBlockOptions);
            var step7 = new TransformBlock<AIImage, AIImage>(UpdateUI, dfBlockOptions);
            var outputBlock = new ActionBlock<AIImage>(result);
            inputBlock.LinkTo(step2, dfLinkOptions);
            step2.LinkTo(step3, dfLinkOptions);
            step3.LinkTo(step4, dfLinkOptions);
            step4.LinkTo(step5, dfLinkOptions);
            step5.LinkTo(step6, dfLinkOptions);
            step6.LinkTo(step7, dfLinkOptions);
            step7.LinkTo(outputBlock, dfLinkOptions);
            return inputBlock;
        }

        private static AIImage UpdateUI(AIImage image)
        {
            App.Current?.Dispatcher.Invoke(() =>
            {
                var grid = new Grid();

                if (image != null)
                {
                    if (image.Paths != null)
                    {
                        grid.Children.Clear();
                        foreach (var path in image.Paths)
                        {
                            grid.Children.Add(path);
                        }
                    }
                }
                image.Grid = grid;
            });
            return image;
        }

        private static async Task<AIImage> CopyOriginalImage(AIImage image)
        {
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.PipelineMat = image.Image.ToMat();
            }, DispatcherPriority);
            return image;
        }

        private static async Task<AIImage> DrawHistogram(AIImage image)
        {
            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.Paths = HistogramDrawer.CalcualteHistogram2(image.PipelineMat);


                //Bitmap bmp = HistogramDrawer.CalcualteHistogram(image.PipelineMat);
                //image.Histogram ??= bmp.ToMat().ToWriteableBitmap();
                //var width = bmp.Width;
                //var height = bmp.Height;
                //BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                //image.Histogram.Lock();
                //NativeMethods.CopyMemory(image.Histogram.BackBuffer, data.Scan0, data.Stride * height);
                //image.Histogram.AddDirtyRect(new Int32Rect(0, 0, width, height));
                //image.Histogram.Unlock();
                //bmp.UnlockBits(data);
                //bmp.Dispose();
            }, DispatcherPriority);
            return image;
        }

        private void UpdateOutput(AIImage image)
        {
            if (image.PipelineMat == null)
            {
                Release();
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                image.AdjusterImage = image.PipelineMat.ToWriteableBitmap();
            }, DispatcherPriority);
            Release();
        }

        private static AIImage NormalizeLABHistogram(AIImage image)
        {
            if (!image.NormalizeLABHistogram)
            {
                return image;
            }
            var labColorMat = new Mat();
            Cv2.CvtColor(image.PipelineMat, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, new(8, 8)).Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(image.PipelineMat, 1 - AIImage.DefaultAutoColorValue, labColorMat, AIImage.DefaultAutoColorValue, 0, image.PipelineMat);
            return image;
        }

        private static AIImage NormalizeRGBHistogram(AIImage image)
        {
            if (!image.NormalizeRGBHistogram)
            {
                return image;
            }
            using var rgbColorMat = image.PipelineMat.Clone();
            var channels = Cv2.Split(rgbColorMat);
            for (var i = 0; i < 3; i++)
            {
                Cv2.CreateCLAHE(2.0, new(8, 8)).Apply(channels[i], channels[i]);
            }
            Cv2.Merge(channels, rgbColorMat);
            Cv2.AddWeighted(image.PipelineMat, 1 - AIImage.DefaultAutoColorValue, rgbColorMat, AIImage.DefaultAutoColorValue, 0, image.PipelineMat);
            foreach (var channel in channels)
            {
                channel.Dispose();
            }
            return image;
        }

        private static AIImage SetBrightness(AIImage image)
        {
            Cv2.ConvertScaleAbs(image.PipelineMat, image.PipelineMat, image.ContrastValue, image.BrightnessValue);
            return image;
        }

        private static AIImage SetExposure(AIImage image)
        {
            var expMat = new Mat();
            Cv2.CvtColor(image.PipelineMat, expMat, ColorConversionCodes.BGR2HSV);
            var channels = Cv2.Split(expMat);
            Cv2.ConvertScaleAbs(channels[2], channels[2], image.ExposureValue);
            Cv2.Merge(channels, expMat);
            Cv2.CvtColor(expMat, image.PipelineMat, ColorConversionCodes.HSV2BGR);
            foreach (var channel in channels)
            {
                channel.Dispose();
            }
            return image;
        }
    }
}
