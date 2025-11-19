using System.Collections.ObjectModel;
using System.Windows.Media;
using PdfEditor.Models;

namespace PdfEditor.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        private ImageSource? _pageImage;
        private ImageSource? _thumbnail;
        private double _pixelWidth;
        private double _pixelHeight;
        private double _pageWidth;
        private double _pageHeight;
        private readonly int? _sourcePageNumber;

        public PageViewModel(PdfPageModel model, int? sourcePageNumber)
        {
            Model = model;
            _sourcePageNumber = sourcePageNumber;
        }

        public PdfPageModel Model { get; }

        public int DisplayPageNumber => Model.PageNumber;

        public int? SourcePageNumber => _sourcePageNumber;

        public ObservableCollection<AnnotationViewModel> Annotations { get; } = new();

        public ImageSource? PageImage
        {
            get => _pageImage;
            set => SetProperty(ref _pageImage, value);
        }

        public ImageSource? Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public double PixelWidth
        {
            get => _pixelWidth;
            set => SetProperty(ref _pixelWidth, value);
        }

        public double PixelHeight
        {
            get => _pixelHeight;
            set => SetProperty(ref _pixelHeight, value);
        }

        public double PageWidth
        {
            get => _pageWidth;
            set => SetProperty(ref _pageWidth, value);
        }

        public double PageHeight
        {
            get => _pageHeight;
            set => SetProperty(ref _pageHeight, value);
        }
    }
}
