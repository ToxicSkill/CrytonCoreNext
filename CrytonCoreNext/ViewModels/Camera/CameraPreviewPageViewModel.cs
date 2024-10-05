﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.Interfaces;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.ViewModels.Camera
{
    public partial class CameraContext : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Models.Camera> _availableCameras;

        [ObservableProperty]
        private Models.Camera _camera;

        public CameraContext()
        {
            AvailableCameras = [];
        }

        partial void OnCameraChanged(Models.Camera value)
        {
            foreach (var camera in AvailableCameras)
            {
                camera.IsSelected = false;
            }
            if (value == null)
            {
                return;
            }
            value.IsSelected = true;
        }
    }

    public partial class CameraPreviewPageViewModel : ObservableObject
    {
        private const int MaxFpsQueueCount = 5;

        private const int DefaultScoreThreshold = 50;

        private const int MilisecondsInSecond = 1000;

        private const int DefaultFps = 5;

        private static readonly DispatcherPriority DispatcherPriority = DispatcherPriority.Background;

        private readonly IYoloModelService _yoloModelService;

        private readonly ISnackbarService _snackbarService;

        private readonly ICameraService _cameraService;

        private CancellationTokenSource _cancellationToken;

        private readonly Queue<int> _fpsQueue;

        [ObservableProperty]
        private int _scoreThreshold = DefaultScoreThreshold;

        [ObservableProperty]
        private bool _runCamera;

        [ObservableProperty]
        private bool _runDetection;

        [ObservableProperty]
        private CameraContext _cameraContext;

        [ObservableProperty]
        private SymbolRegular _currentCameraIcon;

        public CameraPreviewPageViewModel(
            ISnackbarService snackbarService,
            IYoloModelService yoloModelService,
            ICameraService cameraService,
            CameraContext cameraContext)
        {
            _yoloModelService = yoloModelService;
            _cameraService = cameraService;
            _snackbarService = snackbarService;

            _cancellationToken = new CancellationTokenSource();
            _fpsQueue = new Queue<int>();

            CameraContext = cameraContext;
            CurrentCameraIcon = SymbolRegular.Play12;
        }

        [RelayCommand]
        private void StartCamera()
        {
            RunCamera = !RunCamera;
            if (!RunCamera)
            {
                RunDetection = false;
                _cancellationToken.Cancel();
            }
            if (CheckRunCameraConditions())
            {
                _cameraService.SetCurrentCamera(CameraContext.Camera);
                Task.Run(PlayCamera);
            }
        }

        private async Task PlayCamera()
        {
            var fpsMs = MilisecondsInSecond / DefaultFps;
            try
            {
                CurrentCameraIcon = SymbolRegular.Stop20;
                var rect = new Int32Rect();
                var bufferSize = 0;
                while (CameraContext.Camera != null)
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    var timestamp = Stopwatch.GetTimestamp();
                    await Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (RunCamera)
                        {
                            if (_cameraService.GrabCameraFrame())
                            {
                                if (CameraContext.Camera.ImageSource != null)
                                {
                                    CameraContext.Camera.ImageSource.Lock();
                                    CameraContext.Camera.ImageSource.WritePixels(
                                        rect,
                                        RunDetection ?
                                        _yoloModelService.PredictAndDraw(CameraContext.Camera, _cameraService.CurrentImage, ScoreThreshold).Data :
                                        _cameraService.CurrentImage.Data,
                                        bufferSize,
                                        (int)_cameraService.CurrentImage.Step());
                                    CameraContext.Camera.ImageSource.Unlock();
                                }
                                if (CameraContext.Camera.ImageSource == null)
                                {
                                    CameraContext.Camera!.ImageSource = _cameraService.CurrentImage.ToWriteableBitmap();
                                    rect = new Int32Rect(0, 0, _cameraService.CurrentImage.Width, _cameraService.CurrentImage.Height);
                                    bufferSize = _cameraService.CurrentImage.Width * _cameraService.CurrentImage.Height * _cameraService.CurrentImage.Channels();
                                }
                                OnPropertyChanged(nameof(CameraContext.Camera.ImageSource));
                            }
                        }
                    }, DispatcherPriority);
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    var loopTimeMiliseconds = Stopwatch.GetElapsedTime(timestamp, Stopwatch.GetTimestamp()).Milliseconds;
                    if (_fpsQueue.Count > MaxFpsQueueCount / 2)
                    {
                        fpsMs = MilisecondsInSecond / (int)Math.Clamp(MilisecondsInSecond / (_fpsQueue.Average() + 1), 1, CameraContext.Camera!.Fps);
                    }
                    var delayTime = Math.Clamp(fpsMs - loopTimeMiliseconds, 0, fpsMs);
                    CameraContext.Camera!.CurrentFps = Math.Clamp(MilisecondsInSecond / loopTimeMiliseconds, 0, CameraContext.Camera.Fps);
                    AddLoopTimeToQueue(loopTimeMiliseconds);
                    await Task.Delay(delayTime);
                }
            }
            catch (OperationCanceledException)
            {
                RestartCancelToken();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                CurrentCameraIcon = SymbolRegular.Play12;
            }
        }

        private bool CheckRunCameraConditions()
        {
            if (CameraContext.Camera != null)
            {
                return true;
            }
            else
            {
                _snackbarService.Show("Error", "No selected camera", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
                RunCamera = false;
                return false;
            }
        }

        private void AddLoopTimeToQueue(int loopTimeMiliseconds)
        {
            if (_fpsQueue.Count > MaxFpsQueueCount)
            {
                _ = _fpsQueue.Dequeue();
            }
            _fpsQueue.Enqueue(loopTimeMiliseconds);
        }

        private void RestartCancelToken()
        {
            _cancellationToken = new CancellationTokenSource();
        }
    }
}
