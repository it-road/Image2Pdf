# Image2Pdf Console Converter

A simple yet powerful C# console application to convert a folder of images, **now including WebP**, into a single, sorted PDF file.

This tool is designed for ease of use. It interactively prompts the user for necessary information and uses a "natural sort" algorithm to ensure that files like `image1.jpg`, `image2.jpg`, `image10.jpg` are ordered correctly, just as they appear in Windows File Explorer.

## Features

-   **Batch Conversion**: Converts all supported images from a specified folder.
-   **Wide Image Format Support**: Supports common formats like `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tiff`, `.webp`.
-   **Natural Sorting**: Automatically sorts images by filename using the same logic as Windows File Explorer before conversion.
-   **Flexible Output**: Allows the user to specify a full output path, a filename only (saving in the source folder), or use a default.
-   **Conflict Avoidance**: If the default `output.pdf` already exists, it automatically creates a new file named `output (1).pdf`, `output (2).pdf`, and so on, to avoid overwriting existing files.
-   **User-Friendly Interface**: A straightforward command-line interface guides the user through the process.

## How to Use (For End-Users)

1.  Download the `Image2Pdf.exe` file from the project's "Releases" page.
2.  Double-click `Image2Pdf.exe` or run it from a terminal (`cmd` or `powershell`).
3.  Follow the on-screen prompts:
    -   **Enter the path to the image folder**: Paste the full path to the folder containing your images (e.g., `C:\Users\YourName\Desktop\VacationPhotos`).
    -   **Enter the output PDF filename or full path**: You have three options:
        -   **Press Enter (Default)**: Creates `output.pdf` inside your image folder. If it exists, it will create `output (1).pdf`, etc.
        -   **Type a filename**: (e.g., `MyAlbum.pdf`). The PDF will be saved inside your image folder with that name.
        -   **Type a full path**: (e.g., `D:\Documents\MyFinalAlbum.pdf`). The PDF will be saved at that exact location.
4.  The application will process the images and notify you upon completion. Your PDF will be ready at the specified location.