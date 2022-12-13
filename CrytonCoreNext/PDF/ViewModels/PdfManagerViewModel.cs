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

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfManagerViewModel : InteractiveViewBase
    {

        private readonly IPDFService _pdfService;

        private List<PDFFile> _files;

        private List<PDFImage> _images;

        private PDFFile _currentFile;

        public ViewModelBase ExtensionViewModel { get; set; }

        public PDFFile CurrentFile
        {
            get => _currentFile;
            set
            {
                if (_currentFile == value) return;
                _currentFile = value;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }

        public ICommand LoadFilesCommand { get; set; }

        public PdfManagerViewModel(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView, IPDFService pdfService) : base(fileService, dialogService, filesView, progressView)
        {
            _images = new();
            _pdfService = pdfService;
            _files = new();
            LoadFilesCommand = new AsyncCommand(this.LoadPDFFiles, CanExecute);
            FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
        }

        public override void SendObject(object obj)
        {
            if (obj is ViewModelBase viewModel)
            {
                ExtensionViewModel = viewModel;
                OnPropertyChanged(nameof(ExtensionViewModel));
            }
        }

        private async Task LoadPDFFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
            {
                FilesViewModel.AddFile(file);
                var pdfFile = _pdfService.ReadPdf(file);
                if (pdfFile != null)
                {
                    _files.Add(pdfFile);
                    await LoadAllImages(pdfFile);
                }
            }

            FilesViewModel.UpdateFiles();
            Unlock();
        }

        private async Task LoadAllImages(PDFFile file)
        {
            var images = new List<ImageSource>();
            await foreach (var image in _pdfService.LoadAllPDFImages(file))
            {
                images.Add(image);
            }
            _images.Add(new(images, file.Guid));
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

            if (CurrentFile != null && _images != null)
            {
                ExtensionViewModel.SendObject(CurrentFile);
                ExtensionViewModel.SendObject(_images.Where(x => x.Guid == CurrentFile.Guid).ToList());
            }
        }
    }
}
