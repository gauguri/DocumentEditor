using System.Collections.Generic;

namespace PdfEditor.Models
{
    public class PdfDocumentModel
    {
        public string FilePath { get; set; } = string.Empty;
        public List<PdfPageModel> Pages { get; set; } = new List<PdfPageModel>();
    }
}
