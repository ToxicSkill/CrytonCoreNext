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

        public PDFFile CurrentFile
        {
            get => _currentFile;
            set
            {
                if (_currentFile == value) return;
                _currentFile = value;
                Task.Run(() => UpdateImage());
                OnPropertyChanged(nameof(CurrentFile));
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
            FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
        }

        private async Task MovePreviousPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber > 0 ? --pageNumber : 0;
            await UpdateImage();
        }

        private async Task MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber < CurrentFile.NumberOfPages - 1 ? ++pageNumber : CurrentFile.NumberOfPages - 1;
            await UpdateImage();
        }

        private async Task LoadFiles()
        {
            var files = base.LoadFiles(Static.Extensions.DialogFilters.Pdf);
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
        }

        private new void HandleFileChanged(object? sender, EventArgs? e)
        {
            OnPropertyChanged(nameof(CurrentFile));
        }

        private void HandleCurrentFileChanged(object? sender, EventArgs? e)
        {
            var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetCurrentFileGuid());
            if (file == null)
            {
                return;
            }

            if (!file.Guid.Equals(CurrentFile?.Guid))
            {
                CurrentFile = file;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }


        private async Task UpdateImage()
        {
            //var pdf = _pdfService.ReadPdf("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNextTests\\TestingFiles\\1.pdf");
            //var images = _pdfService.GetAllPdfImages(pdf);
            //ImageViewerViewModel = new ImageViewerViewModel(images);

            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Bitmap = _pdfService.GetImage(CurrentFile);
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
