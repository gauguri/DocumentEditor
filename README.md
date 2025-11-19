# PDF Viewer & Editor (WPF)

A lightweight WPF PDF viewer/editor built with .NET 8 and MVVM. The app renders PDFs with PDFium, lets you add annotations (text, highlights, shapes, freehand ink, and comments), insert/delete pages, and save an edited copy.

## Project layout

- `PdfEditor/` – WPF desktop app
  - `Views/` – `MainWindow` with menu bar, thumbnail sidebar, toolbar, zoom controls, and canvas overlay
  - `ViewModels/` – MVVM layer for pages, annotations, and commands
  - `Models/` – DTOs for pages, annotations, tool options
  - `Services/` – Rendering (PDFium) and saving (iText 7) logic plus file dialogs
  - `Converters/` – UI visibility helpers for annotation templates
- `PdfOpener/` – original console sample (left intact)

## Prerequisites

- Windows 10/11
- .NET 8 SDK
- Native PDFium binaries are required at runtime. Because the `PdfiumViewer.Native.*` packages are no longer available on NuGet, download the appropriate `pdfium.dll` from the [official PDFium binaries repository](https://github.com/bblanchon/pdfium-binaries/releases) and place it next to `PdfEditor.exe` (for both x64 and x86 builds, pick the matching architecture).
- NuGet restore will emit a compatibility warning for `PdfiumViewer` because it targets older .NET Framework TFMs; this is expected and the package works on `net8.0-windows` when the native `pdfium.dll` is present.

## Build & run

1. Restore packages:
   ```bash
   dotnet restore PdfEditor/PdfEditor.csproj
   ```
2. Build the WPF app:
   ```bash
   dotnet build PdfEditor/PdfEditor.csproj -c Release
   ```
3. Run (from Windows):
   ```bash
   dotnet run --project PdfEditor/PdfEditor.csproj
   ```
4. Publish a distributable executable (example for x64):
   ```bash
   dotnet publish PdfEditor/PdfEditor.csproj -c Release -r win-x64 --self-contained false
   ```
   The output `PdfEditor.exe` will be under `PdfEditor/bin/Release/net8.0-windows/win-x64/publish/`.
5. Place the downloaded `pdfium.dll` (matching your architecture) in the same folder as `PdfEditor.exe` before running or distributing the app. If you publish `win-x86`, use the x86 PDFium build; for `win-x64`, use the x64 build.

## Features

- Scrollable, zoomable page canvas with thumbnail sidebar
- Toolbar with pointer, highlight, text, rectangle, ellipse, freehand ink, and comment tools
- Insert blank pages after the current selection or delete pages
- Save annotations into a new PDF file using iText 7
- Zoom controls (fit page, fit width, +/- slider)

## Notes on editing

- Highlights and shapes are drawn with a click-and-drag gesture.
- Text and comment annotations are placed with a single click using the values from the toolbar text boxes.
- Freehand ink tracks the mouse path until release.
- Inserted pages inherit the current page size; they render as blank white canvases until annotated.

## Native dependencies

Since the native NuGet bundles are unavailable, you must manually supply `pdfium.dll` beside the built executable. The [pdfium-binaries releases](https://github.com/bblanchon/pdfium-binaries/releases) provide zipped builds for Windows x64 and x86; extract and copy the single `pdfium.dll` that matches your build architecture.
