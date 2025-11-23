using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using PdfEditor.Models;
using PdfEditor.ViewModels;
using PdfPoint = iText.Kernel.Geom.Point;
using SystemPath = System.IO.Path;
using WpfPoint = System.Windows.Point;

namespace PdfEditor.Services
{
    public class PdfEditingService
    {
        public async Task SaveAsync(string sourceFile, string targetFile, IList<PageViewModel> pages)
        {
            var temp = SystemPath.GetTempFileName();

            await Task.Run(() => CopyPagesWithInsertions(sourceFile, temp, pages));
            await Task.Run(() => ApplyAnnotations(temp, targetFile, pages));

            if (File.Exists(temp))
            {
                File.Delete(temp);
            }
        }

        private static void CopyPagesWithInsertions(string sourceFile, string tempFile, IList<PageViewModel> pages)
        {
            using var source = new PdfDocument(new PdfReader(sourceFile));
            using var dest = new PdfDocument(new PdfWriter(tempFile));

            foreach (var page in pages)
            {
                if (page.SourcePageNumber.HasValue)
                {
                    source.CopyPagesTo(page.SourcePageNumber.Value, page.SourcePageNumber.Value, dest);
                }
                else
                {
                    var size = new PageSize((float)page.PageWidth, (float)page.PageHeight);
                    dest.AddNewPage(size);
                }
            }
        }

        private static void ApplyAnnotations(string tempFile, string targetFile, IList<PageViewModel> pages)
        {
            using var pdf = new PdfDocument(new PdfReader(tempFile), new PdfWriter(targetFile));

            for (int i = 0; i < pages.Count; i++)
            {
                var pageVm = pages[i];
                var page = pdf.GetPage(i + 1);
                var canvas = new PdfCanvas(page);
                foreach (var annotation in pageVm.Annotations)
                {
                    var model = annotation.ToModel();
                    switch (model.Type)
                    {
                        case AnnotationType.Text:
                            DrawText(canvas, page, pageVm, model);
                            break;
                        case AnnotationType.Highlight:
                            DrawHighlight(canvas, page, pageVm, model);
                            break;
                        case AnnotationType.Rectangle:
                            DrawRectangle(canvas, page, pageVm, model);
                            break;
                        case AnnotationType.Ellipse:
                            DrawEllipse(canvas, page, pageVm, model);
                            break;
                        case AnnotationType.Freehand:
                            DrawFreehand(canvas, page, pageVm, model);
                            break;
                        case AnnotationType.Comment:
                            AddComment(page, pageVm, model);
                            break;
                    }
                }
            }
        }

        private static void DrawText(PdfCanvas canvas, PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            var start = ToPdfPoint(model.Bounds.TopLeft, pageVm);
            var font = iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            canvas.BeginText();
            canvas.SetFontAndSize(font, 12);
            canvas.MoveText((float)start.GetX(), (float)start.GetY() + 12);
            canvas.SetFillColor(ToDeviceRgb(model.Stroke));
            canvas.ShowText(model.Text);
            canvas.EndText();
        }

        private static void DrawHighlight(PdfCanvas canvas, PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            var rect = ToPdfRect(model.Bounds, pageVm);
            canvas.SaveState();
            canvas.SetFillColor(ToDeviceRgb(model.Fill));
            canvas.Rectangle(rect);
            canvas.Fill();
            canvas.RestoreState();
        }

        private static void DrawRectangle(PdfCanvas canvas, PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            var rect = ToPdfRect(model.Bounds, pageVm);
            canvas.SaveState();
            canvas.SetStrokeColor(ToDeviceRgb(model.Stroke));
            canvas.SetLineWidth((float)model.StrokeThickness);
            canvas.Rectangle(rect);
            canvas.Stroke();
            canvas.RestoreState();
        }

        private static void DrawEllipse(PdfCanvas canvas, PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            var rect = ToPdfRect(model.Bounds, pageVm);
            canvas.SaveState();
            canvas.SetStrokeColor(ToDeviceRgb(model.Stroke));
            canvas.SetLineWidth((float)model.StrokeThickness);
            canvas.Ellipse(rect.GetX(), rect.GetY(), rect.GetX() + rect.GetWidth(), rect.GetY() + rect.GetHeight());
            canvas.Stroke();
            canvas.RestoreState();
        }

        private static void DrawFreehand(PdfCanvas canvas, PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            if (model.Points.Count < 2)
            {
                return;
            }

            canvas.SaveState();
            canvas.SetStrokeColor(ToDeviceRgb(model.Stroke));
            canvas.SetLineWidth((float)model.StrokeThickness);

            var start = ToPdfPoint(model.Points[0], pageVm);
            canvas.MoveTo((float)start.GetX(), (float)start.GetY());
            for (int i = 1; i < model.Points.Count; i++)
            {
                var p = ToPdfPoint(model.Points[i], pageVm);
                canvas.LineTo((float)p.GetX(), (float)p.GetY());
            }

            canvas.Stroke();
            canvas.RestoreState();
        }

        private static void AddComment(PdfPage page, PageViewModel pageVm, AnnotationModel model)
        {
            var rect = ToPdfRect(model.Bounds, pageVm);
            var annotation = new PdfTextAnnotation(rect);
            annotation.SetContents(new iText.Kernel.Pdf.PdfString(model.Text));
            annotation.SetColor(ToDeviceRgb(model.Fill));
            page.AddAnnotation(annotation);
        }

        private static Rectangle ToPdfRect(Rect bounds, PageViewModel pageVm)
        {
            var bottomLeft = ToPdfPoint(new WpfPoint(bounds.X, bounds.Y + bounds.Height), pageVm);
            var width = bounds.Width / pageVm.PixelWidth * pageVm.PageWidth;
            var height = bounds.Height / pageVm.PixelHeight * pageVm.PageHeight;
            return new Rectangle((float)bottomLeft.GetX(), (float)bottomLeft.GetY(), (float)width, (float)height);
        }

        private static PdfPoint ToPdfPoint(WpfPoint point, PageViewModel pageVm)
        {
            var x = point.X / pageVm.PixelWidth * pageVm.PageWidth;
            var y = pageVm.PageHeight - (point.Y / pageVm.PixelHeight * pageVm.PageHeight);
            return new PdfPoint(x, y);
        }

        private static DeviceRgb ToDeviceRgb(System.Windows.Media.Color color) => new(color.R, color.G, color.B);
    }
}
