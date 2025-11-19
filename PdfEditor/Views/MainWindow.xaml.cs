using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PdfEditor.Models;
using PdfEditor.ViewModels;

namespace PdfEditor.Views
{
    public partial class MainWindow : Window
    {
        private Point? _startPoint;
        private AnnotationViewModel? _tempAnnotation;
        private bool _isDrawing;

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (_, __) => Close()));
        }

        private void AnnotationCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedTool == AnnotationType.Pointer || ViewModel.SelectedPage == null)
            {
                return;
            }

            var canvas = sender as Canvas;
            if (canvas == null)
            {
                return;
            }

            _startPoint = ScalePoint(e.GetPosition(canvas));
            _isDrawing = true;

            if (ViewModel.SelectedTool == AnnotationType.Text || ViewModel.SelectedTool == AnnotationType.Comment)
            {
                var annotation = ViewModel.CreateAnnotation(ViewModel.SelectedTool);
                annotation.Bounds = new Rect(_startPoint.Value, new Size(180, 28));
                ViewModel.AddAnnotation(annotation);
                _isDrawing = false;
                _startPoint = null;
                return;
            }

            _tempAnnotation = ViewModel.CreateAnnotation(ViewModel.SelectedTool);
            _tempAnnotation.Bounds = new Rect(_startPoint.Value, new Size(0, 0));
            _tempAnnotation.Points.Clear();
            if (ViewModel.SelectedTool == AnnotationType.Freehand)
            {
                _tempAnnotation.Points.Add(_startPoint.Value);
            }

            ViewModel.AddAnnotation(_tempAnnotation);
        }

        private void AnnotationCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || ViewModel.SelectedTool == AnnotationType.Pointer || ViewModel.SelectedPage == null)
            {
                return;
            }

            var canvas = sender as Canvas;
            if (canvas == null || _startPoint is null || _tempAnnotation is null)
            {
                return;
            }

            var current = ScalePoint(e.GetPosition(canvas));

            if (ViewModel.SelectedTool == AnnotationType.Freehand)
            {
                _tempAnnotation.Points.Add(current);
                return;
            }

            _tempAnnotation.Bounds = NormalizeRect(_startPoint.Value, current);
        }

        private void AnnotationCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            _tempAnnotation = null;
            _startPoint = null;
        }

        private Point ScalePoint(Point point)
        {
            var zoom = ViewModel.ZoomLevel <= 0 ? 1 : ViewModel.ZoomLevel;
            return new Point(point.X / zoom, point.Y / zoom);
        }

        private static Rect NormalizeRect(Point start, Point current)
        {
            var x = Math.Min(start.X, current.X);
            var y = Math.Min(start.Y, current.Y);
            var width = Math.Abs(current.X - start.X);
            var height = Math.Abs(current.Y - start.Y);
            return new Rect(x, y, width, height);
        }
    }
}
