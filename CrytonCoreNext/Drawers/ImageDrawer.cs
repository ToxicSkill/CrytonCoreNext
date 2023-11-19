using CrytonCoreNext.AI.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Org.BouncyCastle.Pqc.Crypto.Frodo;
using System; 
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow; 

namespace CrytonCoreNext.Drawers
{
    public class ImageDrawer
    {
        private const System.Windows.Threading.DispatcherPriority DispatcherPriority = System.Windows.Threading.DispatcherPriority.Send;

        private readonly TransformBlock<AIImage, AIImage> _pipeline;

        private SemaphoreSlim _semaphore;

        public ImageDrawer()
        {
            _semaphore = new SemaphoreSlim(1, 1);
            _pipeline = CreatePipeline((res) => UpdateOutput(res));
        }

        private void Release()
        {
            _semaphore.Release();
        }

        public async Task Post(AIImage image)
        { 
            await _semaphore.WaitAsync();
            if (_pipeline.InputCount == 0)
            { 
                await _pipeline.SendAsync(image);
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
            var outputBlock = new ActionBlock<AIImage>(result);  
            inputBlock.LinkTo(step2, dfLinkOptions);
            step2.LinkTo(step3, dfLinkOptions); 
            step3.LinkTo(step4, dfLinkOptions);
            step4.LinkTo(step5, dfLinkOptions);
            step5.LinkTo(step6, dfLinkOptions);
            step6.LinkTo(outputBlock, dfLinkOptions); 
            return inputBlock;
        }

        private static async Task<AIImage> CopyOriginalImage(AIImage image)
        {
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.PipelineMat = image.Image.ToMat();
            }, DispatcherPriority);
            return image;
        }

        private static async Task<AIImage> DrawHistogram(AIImage image)
        {
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                image.Histogram = HistogramDrawer.CalcualteHistogram(image.PipelineMat);
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
            return image;
        }
    }
}
