using System.Windows.Media;

namespace PdfEditor.Models
{
    public class ToolOptions
    {
        public string TextContent { get; set; } = "Sample text";
        public string CommentContent { get; set; } = "Comment";
        public Color HighlightColor { get; set; } = Color.FromArgb(90, 255, 178, 0);
        public Color StrokeColor { get; set; } = Colors.DeepSkyBlue;
        public Color FillColor { get; set; } = Color.FromArgb(60, 0, 102, 178);
        public double StrokeThickness { get; set; } = 2;
    }
}
