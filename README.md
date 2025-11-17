# PdfOpener

A simple cross-platform .NET console application that opens a PDF file with the system's default viewer.

## Usage

Build and run the application with the path to the PDF you want to open:

```bash
dotnet build PdfOpener

dotnet run --project PdfOpener -- /path/to/document.pdf
```

To produce a platform-specific executable (instead of the framework-dependent DLL), publish the app:

```bash
dotnet publish PdfOpener -c Release
```

The resulting executable will be available at `PdfOpener/bin/Release/net8.0/<OS>/publish/` where `<OS>` matches your platform (for example, `win-x64` or `linux-x64`).

The app validates that the file exists and then uses the platform's default mechanism (`UseShellExecute` on Windows, `open` on macOS, and `xdg-open` on Linux) to open the file.
