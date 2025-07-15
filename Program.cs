using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;        // Required for IComparer<T>
using System.Runtime.InteropServices;   // Required for DllImport
using System.Text;                      // Required for Encoding
using PdfSharp.Drawing;                 // Required for XImage, XGraphics
using PdfSharp.Pdf;                     // Required for PdfDocument, PdfPage

namespace Image2Pdf
{
    // Provides a natural sorting comparison that matches Windows File Explorer.
    // This comparer uses the Windows API function StrCmpLogicalW.
    public class NaturalStringComparer : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Set console encoding to UTF-8 to support non-ASCII characters in file paths.
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- Title Changed as Requested ---
            Console.WriteLine("--- Image to PDF Converter ---");

            // Get the image folder path
            string imageFolderPath;
            while (true)
            {
                Console.Write("Please enter the path to the image folder: ");
                imageFolderPath = Console.ReadLine();
                if (Directory.Exists(imageFolderPath))
                {
                    break;
                }
                Console.WriteLine("Error: The specified folder was not found. Please try again.");
            }

            // Get the output PDF file path (optional)
            Console.Write("Enter the output PDF filename or full path (optional, defaults to 'output.pdf' in the source folder): ");
            string userInputPdfPath = Console.ReadLine();

            string outputPdfPath;

            // If the user did not enter a path, use the default logic.
            if (string.IsNullOrWhiteSpace(userInputPdfPath))
            {
                // Call the helper function to find a unique name.
                outputPdfPath = GetUniquePdfPath(imageFolderPath, "output", ".pdf");
            }
            else
            {
                if (Path.IsPathRooted(userInputPdfPath))
                {
                    outputPdfPath = userInputPdfPath;
                }
                else
                {
                    outputPdfPath = Path.Combine(imageFolderPath, userInputPdfPath);
                }
            }

            // Ensure the file has a .pdf extension
            if (!Path.GetExtension(outputPdfPath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                outputPdfPath = Path.ChangeExtension(outputPdfPath, ".pdf");
            }

            // Get all image files and sort them naturally
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };
            var imageFiles = Directory.GetFiles(imageFolderPath)
                                      .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                                      .OrderBy(f => f, new NaturalStringComparer()) // Use the natural sorter
                                      .ToArray();

            if (imageFiles.Length == 0)
            {
                Console.WriteLine("Error: No supported image files found in the specified folder.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Found {imageFiles.Length} image file(s). Preparing to convert...");
            Console.WriteLine($"PDF will be saved to: {outputPdfPath}");

            try
            {
                // Create the PDF document
                string outputDirectory = Path.GetDirectoryName(outputPdfPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using (PdfDocument document = new PdfDocument())
                {
                    document.Info.Title = Path.GetFileNameWithoutExtension(outputPdfPath);
                    document.Info.Author = "Image2Pdf";

                    foreach (var imageFile in imageFiles)
                    {
                        try
                        {
                            using (XImage image = XImage.FromFile(imageFile))
                            {
                                // Add a new page for each image
                                PdfPage page = document.AddPage();
                                page.Width = image.PointWidth;
                                page.Height = image.PointHeight;

                                XGraphics gfx = XGraphics.FromPdfPage(page);
                                gfx.DrawImage(image, 0, 0, page.Width, page.Height);

                                Console.WriteLine($"Processed: {Path.GetFileName(imageFile)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing file {Path.GetFileName(imageFile)}: {ex.Message}");
                        }
                    }

                    // Save the PDF file
                    document.Save(outputPdfPath);
                }

                Console.WriteLine("\nPDF file created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while creating the PDF: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Generates a unique file path by appending a number if the file already exists.
        /// e.g., output.pdf -> output (1).pdf -> output (2).pdf
        /// </summary>
        /// <param name="directory">The directory where the file will be saved.</param>
        /// <param name="baseName">The initial name of the file without extension.</param>
        /// <param name="extension">The file extension (e.g., ".pdf").</param>
        /// <returns>A unique, non-existent full file path.</returns>
        private static string GetUniquePdfPath(string directory, string baseName, string extension)
        {
            string filePath = Path.Combine(directory, baseName + extension);

            // If the original path doesn't exist, we can use it.
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            // If it exists, start a counter to find the next available name.
            int counter = 1;
            while (true)
            {
                string newFileName = $"{baseName} ({counter}){extension}";
                filePath = Path.Combine(directory, newFileName);

                if (!File.Exists(filePath))
                {
                    // We found a name that doesn't exist, return it.
                    return filePath;
                }

                // This name is also taken, increment and try again in the next loop.
                counter++;
            }
        }
    }
}