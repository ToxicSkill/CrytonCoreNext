﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.ViewModels
{
    [ObservableObject]
    public partial class CameraViewModel
    {
        private const int MaxFpsQueueCount = 10;

        private const int DefaultScoreThreshold = 50;

        private const int MilisecondsInSecond = 1000;

        private const int DefaultFps = 5;

        private static readonly DispatcherPriority DispatcherPriority = DispatcherPriority.Render;

        private readonly ISnackbarService _snackbarService;

        private readonly IYoloModelService _yoloModelService;

        private readonly ICameraService _cameraService;

        private CancellationTokenSource _cancellationToken;

        private readonly Queue<int> _fpsQueue;

        [ObservableProperty]
        public ObservableCollection<Camera> availableCameras;

        [ObservableProperty]
        public Camera selectedCamera;

        [ObservableProperty]
        private bool runCamera;

        [ObservableProperty]
        private bool runDetection;

        [ObservableProperty]
        private int scoreThreshold = DefaultScoreThreshold;

        [ObservableProperty]
        private bool scanInProgress;

        public CameraViewModel(IYoloModelService yoloModelService, ICameraService cameraService, ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;
            _yoloModelService = yoloModelService;
            _cameraService = cameraService;
            _cancellationToken = new CancellationTokenSource();
            _fpsQueue = new Queue<int>();
            AvailableCameras = new();
        }

        partial void OnRunCameraChanged(bool value)
        {
            if (value & CheckRunCameraConditions())
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await PlayCamera();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
            }
            else
            {
                RunDetection = false;
                _cancellationToken.Cancel();
            }
        }

        private bool CheckRunCameraConditions()
        {
            if (SelectedCamera != null)
            {
                return true;
            }
            else
            {
                _snackbarService.Show("Error", "No selected camera", ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
                runCamera = false;
                return false;
            }
        }

        partial void OnSelectedCameraChanged(Camera value)
        {
            _cameraService.SetCurrentCamera(SelectedCamera);
            _cameraService.UpdateCameraInfo(SelectedCamera);
        }

        internal void OnUnloaded()
        {
            RunCamera = false;
        }

        private async Task PlayCamera()
        {
            var fpsMs = MilisecondsInSecond / DefaultFps;
            try
            {
                while (true)
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    var timestamp = Stopwatch.GetTimestamp();
                    await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (RunCamera)
                        {
                            _cameraService.GrabCameraFrame();
                            if (RunDetection)
                            {
                                SelectedCamera.ImageSource = _yoloModelService.PredictAndDraw(SelectedCamera, _cameraService.GetLastCameraFrame(), ScoreThreshold);
                            }
                            else
                            {
                                SelectedCamera.ImageSource = _cameraService.GetLastCameraFrame().ToWriteableBitmap();
                            }
                        }
                    }, DispatcherPriority);
                    var loopTimeMiliseconds = Stopwatch.GetElapsedTime(timestamp, Stopwatch.GetTimestamp()).Milliseconds;
                    if (_fpsQueue.Count > MaxFpsQueueCount / 2)
                    {
                        fpsMs = MilisecondsInSecond / (int)Math.Clamp(MilisecondsInSecond / (_fpsQueue.Average() + 1), 1, SelectedCamera.Fps);
                    }
                    var delayTime = Math.Clamp(fpsMs - loopTimeMiliseconds, 0, fpsMs);
                    SelectedCamera.CurrentFps = Math.Clamp(MilisecondsInSecond / loopTimeMiliseconds, 0, SelectedCamera.Fps);
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

        [RelayCommand]
        private async Task ScanCameras()
        {
            ScanInProgress = true;
            await _cameraService.GetAllConnectedCameras();
            var cameras = _cameraService.GetAllCameras();
            Camera? camera = null;
            if (_cameraService.IsCameraOpen())
            {
                _cameraService.SetBufferSize(0);
                camera = _cameraService.GetCurrentCamera();
            }
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                AvailableCameras = new(cameras);
                if (camera != null)
                {
                    SelectedCamera = camera;
                }
            });
            ScanInProgress = false;
        }
    }
}
