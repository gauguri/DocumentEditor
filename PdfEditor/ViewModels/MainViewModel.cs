using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PdfEditor.Commands;
using PdfEditor.Models;
using PdfEditor.Services;

namespace PdfEditor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly PdfRenderService _renderService;
        private readonly PdfEditingService _editingService;
        private readonly FileDialogService _fileDialogService;
        private ObservableCollection<PageViewModel> _pages = new();
        private PageViewModel? _selectedPage;
        private double _zoomLevel = 1.0;
        private ToolOptions _toolOptions = new();
        private AnnotationType _selectedTool = AnnotationType.Pointer;
        private string _status = "Open a PDF to begin";
        private string? _currentFilePath;

        public MainViewModel()
            : this(new PdfRenderService(), new PdfEditingService(), new FileDialogService())
        {
        }

        public MainViewModel(PdfRenderService renderService, PdfEditingService editingService, FileDialogService fileDialogService)
        {
            _renderService = renderService;
            _editingService = editingService;
            _fileDialogService = fileDialogService;

            OpenCommand = new RelayCommand(async _ => await OpenAsync());
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => Pages.Any());
            InsertPageCommand = new RelayCommand(_ => InsertPageAfterSelection(), _ => SelectedPage != null);
            DeletePageCommand = new RelayCommand(_ => DeleteSelectedPage(), _ => SelectedPage != null);
            ZoomInCommand = new RelayCommand(_ => ZoomLevel += 0.1, _ => ZoomLevel < 3);
            ZoomOutCommand = new RelayCommand(_ => ZoomLevel -= 0.1, _ => ZoomLevel > 0.3);
            FitWidthCommand = new RelayCommand(_ => ZoomLevel = 1.2, _ => SelectedPage != null);
            FitPageCommand = new RelayCommand(_ => ZoomLevel = 1.0, _ => SelectedPage != null);
            SetToolCommand = new RelayCommand(param =>
            {
                if (param is AnnotationType tool)
                {
                    SelectedTool = tool;
                }
            });
        }

        public ObservableCollection<PageViewModel> Pages
        {
            get => _pages;
            private set => SetProperty(ref _pages, value);
        }

        public PageViewModel? SelectedPage
        {
            get => _selectedPage;
            set => SetProperty(ref _selectedPage, value);
        }

        public double ZoomLevel
        {
            get => _zoomLevel;
            set => SetProperty(ref _zoomLevel, Math.Clamp(value, 0.25, 4));
        }

        public ToolOptions ToolOptions
        {
            get => _toolOptions;
            set => SetProperty(ref _toolOptions, value);
        }

        public AnnotationType SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand InsertPageCommand { get; }
        public ICommand DeletePageCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand FitWidthCommand { get; }
        public ICommand FitPageCommand { get; }
        public ICommand SetToolCommand { get; }

        public async Task OpenAsync()
        {
            var file = _fileDialogService.SelectPdfToOpen();
            if (string.IsNullOrWhiteSpace(file))
            {
                return;
            }

            _currentFilePath = file;
            Status = "Loading...";
            var document = await _renderService.LoadAsync(file);

            var pages = new ObservableCollection<PageViewModel>();
            foreach (var pageModel in document.Pages)
            {
                var viewModel = new PageViewModel(pageModel, pageModel.PageNumber);
                await _renderService.RenderPageAsync(pageModel.PageNumber, viewModel, 1.0);
                await _renderService.RenderThumbnailAsync(pageModel.PageNumber, viewModel, 0.2);
                pages.Add(viewModel);
            }

            Pages = pages;
            SelectedPage = Pages.FirstOrDefault();
            Status = $"Loaded {Pages.Count} pages";
        }

        public void InsertPageAfterSelection()
        {
            if (SelectedPage == null)
            {
                return;
            }

            var insertIndex = Pages.IndexOf(SelectedPage) + 1;
            var model = new PdfPageModel
            {
                PageNumber = insertIndex + 1,
                Width = SelectedPage.PageWidth,
                Height = SelectedPage.PageHeight,
                IsNewPage = true
            };

            var vm = new PageViewModel(model, null)
            {
                PageWidth = model.Width,
                PageHeight = model.Height,
                PixelWidth = SelectedPage.PixelWidth,
                PixelHeight = SelectedPage.PixelHeight,
                PageImage = _renderService.CreateBlankPage(model.Width, model.Height),
                Thumbnail = _renderService.CreateBlankThumbnail(model.Width, model.Height)
            };

            Pages.Insert(insertIndex, vm);
            RenumberPages();
            SelectedPage = vm;
        }

        public void DeleteSelectedPage()
        {
            if (SelectedPage == null)
            {
                return;
            }

            Pages.Remove(SelectedPage);
            RenumberPages();
            SelectedPage = Pages.FirstOrDefault();
        }

        private void RenumberPages()
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].Model.PageNumber = i + 1;
                Pages[i].OnPropertyChanged(nameof(PageViewModel.DisplayPageNumber));
            }

            Status = $"Pages: {Pages.Count}";
        }

        public AnnotationViewModel CreateAnnotation(AnnotationType type)
        {
            var annotation = new AnnotationViewModel(type)
            {
                Stroke = ToolOptions.StrokeColor,
                Fill = type == AnnotationType.Highlight ? ToolOptions.HighlightColor : ToolOptions.FillColor,
                StrokeThickness = ToolOptions.StrokeThickness,
                Text = type == AnnotationType.Text ? ToolOptions.TextContent : ToolOptions.CommentContent
            };

            return annotation;
        }

        public void AddAnnotation(AnnotationViewModel annotation)
        {
            SelectedPage?.Annotations.Add(annotation);
        }

        public async Task SaveAsync()
        {
            if (_currentFilePath is null || !Pages.Any())
            {
                return;
            }

            var targetPath = _fileDialogService.SelectPdfToSave();
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                return;
            }

            Status = "Saving...";
            await _editingService.SaveAsync(_currentFilePath, targetPath, Pages.ToList());
            Status = $"Saved to {targetPath}";
        }
    }
}
