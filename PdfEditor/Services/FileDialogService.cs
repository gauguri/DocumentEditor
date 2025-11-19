using Microsoft.Win32;

namespace PdfEditor.Services
{
    public class FileDialogService
    {
        public string? SelectPdfToOpen()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = false
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? SelectPdfToSave()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                DefaultExt = "pdf"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
