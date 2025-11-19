using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using PdfEditor.Models;
using PdfEditor.ViewModels;
using PdfiumViewer;

namespace PdfEditor.Services
{
    public class PdfRenderService : IDisposable
    {
        private PdfDocument? _document;

        public async Task<PdfDocumentModel> LoadAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("PDF not found", path);
            }

            _document?.Dispose();
            _document = await Task.Run(() => PdfDocument.Load(path));

            var model = new PdfDocumentModel { FilePath = path };
            for (int i = 0; i < _document.PageCount; i++)
            {
                var size = _document.PageSizes[i];
                model.Pages.Add(new PdfPageModel
                {
                    PageNumber = i + 1,
                    Width = size.Width,
                    Height = size.Height
                });
            }

            return model;
        }

        public async Task RenderPageAsync(int pageNumber, PageViewModel viewModel, double scale)
        {
            if (_document is null)
            {
                throw new InvalidOperationException("PDF is not loaded");
            }

            var size = _document.PageSizes[pageNumber - 1];
            int width = (int)(size.Width * 96 / 72 * scale);
            int height = (int)(size.Height * 96 / 72 * scale);

            var bitmap = await Task.Run(() => _document.Render(pageNumber - 1, width, height, 96, 96, PdfRenderFlags.ForPrinting));
            viewModel.PageImage = ToBitmapSource(bitmap);
            viewModel.PixelWidth = width;
            viewModel.PixelHeight = height;
            viewModel.PageWidth = size.Width;
            viewModel.PageHeight = size.Height;
        }

        public async Task RenderThumbnailAsync(int pageNumber, PageViewModel viewModel, double scale)
        {
            if (_document is null)
            {
                throw new InvalidOperationException("PDF is not loaded");
            }

            var size = _document.PageSizes[pageNumber - 1];
            int width = (int)(size.Width * 96 / 72 * scale);
            int height = (int)(size.Height * 96 / 72 * scale);
            var bitmap = await Task.Run(() => _document.Render(pageNumber - 1, Math.Max(width, 60), Math.Max(height, 80), 96, 96, PdfRenderFlags.Annotations));
            viewModel.Thumbnail = ToBitmapSource(bitmap);
        }

        public BitmapSource CreateBlankPage(double pageWidth, double pageHeight)
        {
            int width = (int)(pageWidth * 96 / 72);
            int height = (int)(pageHeight * 96 / 72);
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            return ToBitmapSource((Bitmap)bmp.Clone());
        }

        public BitmapSource CreateBlankThumbnail(double pageWidth, double pageHeight)
        {
            int width = (int)(pageWidth * 0.25);
            int height = (int)(pageHeight * 0.25);
            width = Math.Max(width, 60);
            height = Math.Max(height, 80);
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            return ToBitmapSource((Bitmap)bmp.Clone());
        }

        private static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            var handle = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                NativeMethods.DeleteObject(handle);
                bitmap.Dispose();
            }
        }

        public void Dispose()
        {
            _document?.Dispose();
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
