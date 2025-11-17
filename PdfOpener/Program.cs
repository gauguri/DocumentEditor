using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PdfOpener;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: PdfOpener <path-to-pdf>");
            return 1;
        }

        var pdfPath = args[0];
        var fullPath = Path.GetFullPath(pdfPath);

        if (!File.Exists(fullPath))
        {
            Console.Error.WriteLine($"PDF not found at '{fullPath}'.");
            return 1;
        }

        try
        {
            OpenPdf(fullPath);
            Console.WriteLine($"Opening '{fullPath}' with the system default PDF viewer.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to open PDF: {ex.Message}");
            return 1;
        }
    }

    private static void OpenPdf(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
            return;
        }

        var opener = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "open" : "xdg-open";

        Process.Start(new ProcessStartInfo
        {
            FileName = opener,
            ArgumentList = { path },
            UseShellExecute = false
        });
    }
}
