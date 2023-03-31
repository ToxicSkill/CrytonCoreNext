﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {
        private readonly IFileService _fileService;

        private readonly ISnackbarService _snackbarService;

        private readonly IPDFService _pdfService;

        [ObservableProperty]
        public ObservableCollection<PDFFile> files;

        [ObservableProperty]
        public ObservableCollection<ImageFile> imageFiles;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Files))]
        public PDFFile selectedPdfFile;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageFiles))]
        public ImageFile selectedImageFile;

        public PdfViewModel(IPDFService pdfService,
            IFileService fileService,
            IDialogService dialogService,
            ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
            _snackbarService = snackbarService;
            _pdfService = pdfService;
            files = new();
            imageFiles = new();
        }

        partial void OnSelectedPdfFileChanged(PDFFile value)
        {
            value.PageImage = _pdfService.LoadImage(value);
        }

        [RelayCommand]
        private new async Task LoadPdfFiles()
        {
            Lock();
            var protectedFile = new List<File>();
            await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
            {
                var pdfFile = _pdfService.ReadPdf(file);
                if (pdfFile != null)
                {
                    Files.Add(pdfFile);
                }
                else
                {
                    protectedFile.Add(file);
                }
                SelectedPdfFile = Files.Last();
            }
            if (SelectedPdfFile == null && Files.Any())
            {
                SelectedPdfFile = Files.First();
            }
            if (protectedFile.Any())
            {
                _snackbarService.Show("Warning",
                    (protectedFile.Count > 1 ?
                    $"{protectedFile.Count} of {protectedFile.Count + files.Count} loaded files" :
                    "One file") + " require password",
                    Wpf.Ui.Common.SymbolRegular.Warning20,
                    Wpf.Ui.Common.ControlAppearance.Caution);
            }
            Unlock();
        }


        [RelayCommand]
        private new async Task LoadImageFiles()
        {
            Lock();
            await foreach (var imageFile in base.LoadFiles(Static.Extensions.DialogFilters.Images))
            {
                ImageFiles.Add(new ImageFile(imageFile));
                SelectedImageFile = ImageFiles.Last();
            }
            if (SelectedImageFile == null && ImageFiles.Any())
            {
                SelectedImageFile = ImageFiles.First();
            }
            Unlock();
        }
    }
}
