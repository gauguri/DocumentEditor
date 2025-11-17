# PdfOpener

A simple cross-platform .NET console application that opens a PDF file with the system's default viewer.

## Usage

Build and run the application with the path to the PDF you want to open:

```bash
dotnet build PdfOpener

dotnet run --project PdfOpener -- /path/to/document.pdf
```

The app validates that the file exists and then uses the platform's default mechanism (`UseShellExecute` on Windows, `open` on macOS, and `xdg-open` on Linux) to open the file.
