using CrytonCoreNext.AI.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CrytonCoreNext.Drawers
{
    public class ImageDrawer
    {
        private const System.Windows.Threading.DispatcherPriority DispatcherPriority = System.Windows.Threading.DispatcherPriority.Send;

        private readonly TransformBlock<AIImage, AIImage> _pipeline;


        public ImageDrawer()
        {
            _pipeline = CreatePipeline();
        }

        public void Post(AIImage image)
        {
            _pipeline.Post(image);
        }

        public static TransformBlock<AIImage, AIImage> CreatePipeline()
        {
            var dfBlockOptions = new ExecutionDataflowBlockOptions();
            var dfLinkOptions = new DataflowLinkOptions();
            var inputBlock = new TransformBlock<AIImage, AIImage>(CopyOriginalImage, dfBlockOptions);
            var step2 = new TransformBlock<AIImage, AIImage>(SetAutoColor, dfBlockOptions);
            var step3 = new TransformBlock<AIImage, AIImage>(SetBrightness, dfBlockOptions);
            var step4 = new TransformBlock<AIImage, AIImage>(SetHistogram, dfBlockOptions); 
            var outputBlock = new ActionBlock<AIImage>(PrepareForOutput);
            inputBlock.LinkTo(step2, dfLinkOptions);
            step2.LinkTo(step3, dfLinkOptions);
            step3.LinkTo(step4, dfLinkOptions);
            step4.LinkTo(outputBlock, dfLinkOptions);
            return inputBlock;
        }

        private static async Task<AIImage> SetHistogram(AIImage image)
        {
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.Histogram = HistogramDrawer.CalcualteHistogram(image.PipelineMat);
            }, DispatcherPriority);
            return image;
        }

        private static async Task<AIImage> CopyOriginalImage(AIImage image)
        {
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.PipelineMat = image.Image.ToMat();
            }, DispatcherPriority);
            return image;
        }

        private static async Task PrepareForOutput(AIImage image)
        {
            await App.Current.Dispatcher.BeginInvoke(() =>
            {
                image.AdjusterImage = image.PipelineMat.ToWriteableBitmap();
            }, DispatcherPriority);
        }

        private static AIImage SetAutoColor(AIImage image)
        {
            if (!image.UseAutoColor)
            {
                return image;
            }
            using var labColorMat = new Mat();
            Cv2.CvtColor(image.PipelineMat, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, new(8, 8)).Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(image.PipelineMat, 1 - AIImage.DefaultAutoColorValue, labColorMat, AIImage.DefaultAutoColorValue, 0, image.PipelineMat); 
            return image;
        }

        private static AIImage SetBrightness(AIImage image)
        { 
            Cv2.ConvertScaleAbs(image.PipelineMat, image.PipelineMat, image.ContrastValue, image.BrightnessValue);
            return image;
        }

        private static AIImage SetExposure(AIImage image)
        { 
            Cv2.AddWeighted(image.PipelineMat, 1, image.PipelineMat, 1 + 0, 1, image.PipelineMat);
            return image;
        }
    }
}
