using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PdfEditor.Models
{
    public class AnnotationModel
    {
        public AnnotationType Type { get; set; }
        public Rect Bounds { get; set; }
        public IList<Point> Points { get; set; } = new List<Point>();
        public string Text { get; set; } = string.Empty;
        public Color Stroke { get; set; } = Colors.DeepSkyBlue;
        public Color Fill { get; set; } = Color.FromArgb(90, 255, 178, 0);
        public double StrokeThickness { get; set; } = 2;
    }
}
