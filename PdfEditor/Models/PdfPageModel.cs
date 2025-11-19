using System.Windows;

namespace PdfEditor.Models
{
    public class PdfPageModel
    {
        public int PageNumber { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsNewPage { get; set; }
    }
}
