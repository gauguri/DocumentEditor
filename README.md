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
- Native PDFium binaries are pulled automatically via the `PdfiumViewer.Native` packages (x64/x86). No extra manual steps are required beyond `dotnet restore` on Windows. If you publish self-contained, ensure you target `win-x64` or `win-x86` so the packaged native binaries are copied to the output.

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

The `PdfiumViewer.Native` NuGet packages embed the required PDFium binaries for Windows x86/x64. When publishing, ensure you target the appropriate runtime identifier so the matching native DLLs are copied alongside the executable.
