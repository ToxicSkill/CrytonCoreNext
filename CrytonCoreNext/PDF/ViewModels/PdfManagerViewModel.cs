using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfManagerViewModel : InteractiveViewBase
    {
        //private readonly IPDFService _pdfService;

        //private List<PDFFile> _files;

        //private List<PDFImage> _images;

        //private PDFFile _currentFile;

        //public ViewModelBase ExtensionViewModel { get; set; }

        //public PDFFile CurrentFile
        //{
        //    get => _currentFile;
        //    set
        //    {
        //        if (_currentFile == value) return;
        //        _currentFile = value;
        //        OnPropertyChanged(nameof(CurrentFile));
        //    }
        //}

        //public ICommand LoadFilesCommand { get; set; }

        //public ICommand SaveFileCommand { get; set; }

        //public PdfManagerViewModel(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView, IPDFService pdfService) : base(fileService, dialogService, filesView, progressView)
        //{
        //    _pdfService = pdfService;

        //    _images = new();
        //    _files = new();

        //    LoadFilesCommand = new AsyncCommand(this.LoadPDFFiles, CanExecute);
        //    SaveFileCommand = new Command(this.SavePDFFile, CanExecute);

        //    FilesViewModel.CurrentFileChanged += HandleCurrentFileChanged;
        //    FilesViewModel.FileDeleted += HandleFileDeleted;
        //    FilesViewModel.AllFilesDeleted += HandleAllFilesDeleted;
        //    FilesViewModel.FilesReordered += HandleFilesReordered;
        //}

        //private void SavePDFFile()
        //{
        //    base.SaveFile(CurrentFile);
        //}

        //public override void SendObject(object obj)
        //{
        //    if (obj is ViewModelBase viewModel)
        //    {
        //        ExtensionViewModel = viewModel;
        //        NotifyExtensionViewModel();
        //        OnPropertyChanged(nameof(ExtensionViewModel));
        //    }
        //    if (obj is string pageName)
        //    {
        //        PageName = pageName;
        //        OnPropertyChanged(nameof(PageName));
        //    }
        //    if (obj is File file)
        //    {
        //        _ = CreateLegacyFile(file);
        //    }
        //}

        //private async Task CreateLegacyFile(File file)
        //{
        //    await AddFile(file);
        //    FilesViewModel.UpdateFiles();
        //}

        //private async Task LoadPDFFiles()
        //{
        //    Lock();
        //    await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
        //    {
        //        await AddFile(file);
        //    }

        //    FilesViewModel.UpdateFiles();
        //    GCHelper.Collect();
        //    Unlock();
        //}

        //private async Task AddFile(File file)
        //{
        //    FilesViewModel.AddFile(file);
        //    var pdfFile = _pdfService.ReadPdf(file);
        //    if (pdfFile != null)
        //    {
        //        _files.Add(pdfFile);
        //        await LoadAllImages(pdfFile);
        //    }
        //}

        //private async Task LoadAllImages(PDFFile file)
        //{
        //    var images = new List<ImageSource>();
        //    await foreach (var image in _pdfService.LoadAllPDFImages(file))
        //    {
        //        images.Add(image);
        //    }
        //    _images.Add(new(images, file.Guid));
        //}

        //private void HandleFilesReordered(object? sender, EventArgs e)
        //{
        //    var orderFiles = FilesViewModel.GetFilesOrder();
        //    _files = _files.OrderBy(x => orderFiles[x.Guid]).ToList();
        //    ExtensionViewModel.SendObject(_files);
        //}

        //private void HandleFileChanged(object? sender, EventArgs? e)
        //{
        //    OnPropertyChanged(nameof(CurrentFile));
        //}

        //private void HandleAllFilesDeleted(object? sender, EventArgs e)
        //{
        //    _files.Clear();
        //    ExtensionViewModel.SendObject(false);
        //}

        //private void HandleFileDeleted(object? sender, EventArgs e)
        //{
        //    var deletedFileGuid = FilesViewModel.GetDeletedFileGuid();
        //    _files.Remove(_files.Select(x => x).Where(x => x.Guid == deletedFileGuid).First());
        //}

        //private void HandleCurrentFileChanged(object? sender, EventArgs? e)
        //{
        //    var file = _files.FirstOrDefault(x => x?.Guid == FilesViewModel.GetCurrentFileGuid());
        //    if (file == null)
        //    {
        //        return;
        //    }

        //    if (!file.Guid.Equals(CurrentFile?.Guid))
        //    {
        //        CurrentFile = file;
        //        OnPropertyChanged(nameof(CurrentFile));
        //    }
        //    NotifyExtensionViewModel();
        //}

        //private void NotifyExtensionViewModel()
        //{
        //    if (CurrentFile != null && _images != null)
        //    {
        //        ExtensionViewModel.SendObject(CurrentFile);
        //        ExtensionViewModel.SendObject(_files);
        //        ExtensionViewModel.SendObject(_images.Where(x => x.Guid == CurrentFile.Guid).ToList());
        //    }
        //}
        public PdfManagerViewModel(IFileService fileService, CrytonCoreNext.Interfaces.IDialogService dialogService, ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
        }
    }
}
