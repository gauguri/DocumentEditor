using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PdfEditor.Models;

namespace PdfEditor.ViewModels
{
    public class AnnotationViewModel : ViewModelBase
    {
        private Rect _bounds;
        private string _text = string.Empty;
        private Color _stroke = Colors.DeepSkyBlue;
        private Color _fill = Color.FromArgb(90, 255, 178, 0);
        private double _strokeThickness = 2;

        public AnnotationViewModel(AnnotationType type)
        {
            Type = type;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public AnnotationType Type { get; }

        public Rect Bounds
        {
            get => _bounds;
            set => SetProperty(ref _bounds, value);
        }

        public PointCollection Points { get; } = new();

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public Color Stroke
        {
            get => _stroke;
            set => SetProperty(ref _stroke, value);
        }

        public Color Fill
        {
            get => _fill;
            set => SetProperty(ref _fill, value);
        }

        public double StrokeThickness
        {
            get => _strokeThickness;
            set => SetProperty(ref _strokeThickness, value);
        }

        public AnnotationModel ToModel() => new()
        {
            Type = Type,
            Bounds = Bounds,
            Points = Points.ToList(),
            Text = Text,
            Fill = Fill,
            Stroke = Stroke,
            StrokeThickness = StrokeThickness
        };
    }
}
