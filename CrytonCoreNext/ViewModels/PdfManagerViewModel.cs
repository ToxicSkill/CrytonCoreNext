using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

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
                LoadCurrentImage();
                OnPropertyChanged(nameof(CurrentFile));
            }
        }

        private readonly IPDFService _pdfService;

        public ImageSource Bitmap { get; set; }

        public ICommand PostFilesCommand { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public IImageView ImageViewerViewModel { get; init; }

        public PdfManagerViewModel(IFileService fileService, IDialogService dialogService, IFilesView filesView, IImageView imageView, IProgressView progressView, IPDFService pdfService) : base(fileService, dialogService, filesView, progressView)
        {
            PostFilesCommand = new AsyncCommand(this.LoadPDFFiles, CanExecute);
            PreviousCommand = new Command(MovePreviousPage, CanExecute);
            NextCommand = new Command(MoveNextPage, CanExecute);
            _pdfService = pdfService;
            ImageViewerViewModel = imageView;
            _files = new();
            FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
            OnPropertyChanged(nameof(ImageViewerViewModel));
        }

        private void MovePreviousPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber > 0 ? --pageNumber : 0;
            LoadCurrentImage();
        }

        private void MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber < CurrentFile.NumberOfPages - 1 ? ++pageNumber : CurrentFile.NumberOfPages - 1;
            LoadCurrentImage();
        }

        private async Task LoadPDFFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
            {
                FilesViewModel.AddFile(file);
                _files.Add(_pdfService.ReadPdf(file) ?? null);
            }

            FilesViewModel.UpdateFiles();
            await LoadAllImages();
            Unlock();
            LoadCurrentImage();
        }

        private void LoadCurrentImage()
        {
            var image = ImageViewerViewModel.GetPDFImage(CurrentFile.LastPage);
            if (image != null)
            {
                Bitmap = image.Source;
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        private async Task LoadAllImages()
        {
            if (CurrentFile == null) return;
            await foreach (var (image, index) in _pdfService.LoadAllPDFImages(CurrentFile))
            {
                ImageViewerViewModel.Add(new(image, index));
            }
        }

        private void HandleFileChanged(object? sender, EventArgs? e)
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


        //private async Task UpdateImage()
        //{
        //    //var pdf = _pdfService.ReadPdf("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNextTests\\TestingFiles\\1.pdf");
        //    //var images = _pdfService.GetAllPdfImages(pdf);
        //    //ImageViewerViewModel = new ImageViewerViewModel(images);

        //    //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        //    //{
        //    //    Bitmap = _pdfService.GetImage(CurrentFile);
        //    //    //Bitmap = new(_currentPDFImages[CurrentPDFFile.LastPage]);
        //    //    OnPropertyChanged(nameof(Bitmap));
        //    //}));
        //}
    }
}
