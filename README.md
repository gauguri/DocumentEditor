# PdfOpener

A simple cross-platform .NET console application that opens and previews a PDF file without relying on external viewers.

## Usage

Build and run the application with the path to the PDF you want to open:

```bash
dotnet build PdfOpener

dotnet run --project PdfOpener -- /path/to/document.pdf
```

Once a PDF is loaded, the app provides an interactive reader:

- `n` or **Enter**: move to the next page
- `p`: move to the previous page
- A page number (e.g., `5`): jump directly to that page
- `q`: quit the viewer

The app extracts text from each page and wraps it for easy reading. If a page does not contain text (for example, image-only pages), the viewer will indicate that no extractable text is available.

To produce a platform-specific executable (instead of the framework-dependent DLL), publish the app:

```bash
dotnet publish PdfOpener -c Release
```

The resulting executable will be available at `PdfOpener/bin/Release/net8.0/<OS>/publish/` where `<OS>` matches your platform (for example, `win-x64` or `linux-x64`).

The app validates that the file exists and then loads its content directly using `UglyToad.PdfPig`, avoiding the system's default PDF viewer.
