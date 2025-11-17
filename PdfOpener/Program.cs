using System.Text;
using UglyToad.PdfPig;

namespace PdfOpener;

internal static class Program
{
    private const int DefaultWrapWidth = 100;

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
            using var document = PdfDocument.Open(fullPath);
            RunViewer(document, fullPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to open PDF: {ex.Message}");
            return 1;
        }
    }

    private static void RunViewer(PdfDocument document, string fullPath)
    {
        Console.WriteLine($"Loaded '{fullPath}' ({document.NumberOfPages} pages).");
        Console.WriteLine("Commands: [n]ext, [p]revious, page number, or [q]uit.");

        var pageNumber = 1;

        while (true)
        {
            DisplayPage(document, pageNumber);
            Console.Write("Command [n/p/number/q]: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) || input.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber < document.NumberOfPages)
                {
                    pageNumber++;
                }
                else
                {
                    Console.WriteLine("Already at the last page.");
                }

                continue;
            }

            if (input.Equals("p", StringComparison.OrdinalIgnoreCase))
            {
                if (pageNumber > 1)
                {
                    pageNumber--;
                }
                else
                {
                    Console.WriteLine("Already at the first page.");
                }

                continue;
            }

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exiting viewer.");
                return;
            }

            if (int.TryParse(input, out var requestedPage))
            {
                if (requestedPage < 1 || requestedPage > document.NumberOfPages)
                {
                    Console.WriteLine($"Enter a page number between 1 and {document.NumberOfPages}.");
                }
                else
                {
                    pageNumber = requestedPage;
                }

                continue;
            }

            Console.WriteLine("Unknown command. Use n, p, q, or a page number.");
        }
    }

    private static void DisplayPage(PdfDocument document, int pageNumber)
    {
        var page = document.GetPage(pageNumber);
        var text = page.Text ?? string.Empty;

        Console.WriteLine();
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"Page {pageNumber}/{document.NumberOfPages}");
        Console.WriteLine(new string('-', 80));

        if (string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("[This page has no extractable text.]");
            Console.WriteLine();
            return;
        }

        foreach (var line in WrapText(text, DefaultWrapWidth))
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
    }

    private static IEnumerable<string> WrapText(string text, int wrapWidth)
    {
        var lineBuilder = new StringBuilder();
        var words = text.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            var willWrap = lineBuilder.Length > 0 && lineBuilder.Length + word.Length + 1 > wrapWidth;

            if (willWrap)
            {
                yield return lineBuilder.ToString();
                lineBuilder.Clear();
            }

            if (lineBuilder.Length > 0)
            {
                lineBuilder.Append(' ');
            }

            lineBuilder.Append(word);
        }

        if (lineBuilder.Length > 0)
        {
            yield return lineBuilder.ToString();
        }
    }
}
