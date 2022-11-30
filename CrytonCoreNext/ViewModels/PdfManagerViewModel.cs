using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CrytonCoreNext.ViewModels
{
    public class PdfManagerViewModel : InteractiveViewBase
    {
        private List<PDFFile> _files;

        private PDFFile _currentFile;
        public PDFFile CurrentPDFFile
        {
            get => _currentFile;
            set
            {
                if (_currentFile == value) return;
                _currentFile = value;
                Task.Run(() => UpdateImage());
                OnPropertyChanged(nameof(CurrentPDFFile));
            }
        }

        private readonly IPDFService _pdfService;

        public WriteableBitmap Bitmap { get; set; }

        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ViewModelBase ImageViewerViewModel { get; set; }

        public PdfManagerViewModel(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView, IPDFService pdfService) : base(fileService, dialogService, filesView, progressView)
        {
            PostFilesCommand = new AsyncCommand(this.LoadFiles, CanExecute);
            PreviousCommand = new AsyncCommand(MovePreviousPage, CanExecute);
            NextCommand = new AsyncCommand(MoveNextPage, CanExecute);
            _pdfService = pdfService;
            _files = new();
            FilesViewModel.FilesChanged += HandleFileChanged;
        }

        private async Task MovePreviousPage()
        {
            var pageNumber = CurrentPDFFile.LastPage;
            CurrentPDFFile.LastPage = pageNumber > 0 ? --pageNumber : 0;
            await UpdateImage();
        }

        private async Task MoveNextPage()
        {
            var pageNumber = CurrentPDFFile.LastPage;
            CurrentPDFFile.LastPage = pageNumber < CurrentPDFFile.NumberOfPages - 1 ? ++pageNumber : CurrentPDFFile.NumberOfPages - 1;
            await UpdateImage();
        }


        private async Task LoadFiles()
        {
            var files = LoadFiles(Static.Extensions.DialogFilters.Pdf);
            foreach (var file in files)
            {
                if (_files.Select(x => x.Guid == file.Guid).FirstOrDefault() == null)
                {
                    continue;
                }
                var pdfFile = _pdfService.ReadPdf(file) ?? null;
                if (pdfFile != null)
                {
                    _files.Add(pdfFile);
                }
            }

            FilesViewModel.UpdateFiles(files);
            UpdateCurrentFile();
            await UpdateImage();
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            UpdateCurrentFile();
            //var images = _pdfService.GetAllPdfImages(pdf);
        }


        private void UpdateCurrentFile()
        {
            if (!_files.Any() || _currentFile == null)
            {
                return;
            }

            CurrentPDFFile = _files.FirstOrDefault(x => x.Guid == _currentFile.Guid);
        }

        private async Task UpdateImage()
        {
            //var pdf = _pdfService.ReadPdf("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNextTests\\TestingFiles\\1.pdf");
            //var images = _pdfService.GetAllPdfImages(pdf);
            //ImageViewerViewModel = new ImageViewerViewModel(images);

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Bitmap = _pdfService.GetImage(CurrentPDFFile);
                //Bitmap = new(_currentPDFImages[CurrentPDFFile.LastPage]);
                OnPropertyChanged(nameof(Bitmap));
            }));
        }


        private bool CanExecute()
        {
            return true;
        }
    }
}
