using System.Drawing;
using Spire.Doc;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using TestGenerator.DAL.Models;
using FileFormat = Spire.Doc.FileFormat;

namespace TestGenerator.Web.Services;

public class FileProcessor : IFileProcessor
{
    private const string UploadFolderName = "Uploaded";

    public async Task<string> GetTextFromFileAsync(IFormFile file)
    {
        try
        {
            if (!IsFileValid(file))
            {
                throw new Exception("File not valid!");
            }

            await using (var stream = file.OpenReadStream())
            {
                var document = new Document();
                await Task.Run(() => document.LoadFromStream(stream, FileFormat.Auto));
                var text = document.GetText();
                return text.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.\r\n", "");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Message {ex.Message}");
        }
    }

    /// <summary>
    ///     Uploads the file to the server and returns true if the file is uploaded successfully.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="newFileName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> UploadFile(IFormFile file)
    {
        bool isCopied;

        try
        {
            if (!IsFileValid(file))
            {
                return false;
            }

            var filePath = GetPath(file);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            isCopied = true;
        }
        catch (Exception ex)
        {
            //TempData["Message"] = $"An unknown error occurred! Please try again. If the problem persists contact support.";
            throw new Exception($"Message {ex.Message}");
        }

        return isCopied;
    }

    public MemoryStream GeneratePdf(Test test)
    {
        // Create a new PDF document
        var pdfDoc = new PdfDocument();

        // Add a new page to the document
        var page = pdfDoc.Pages.Add();

        // Create a PDF brush for text color
        var brush = new PdfSolidBrush(Color.Black);

        // Create a PDF font for text formatting
        var font = new PdfTrueTypeFont(new Font("Arial", 14f), true);

        // Set the starting y-coordinate for the text
        var y = 50f;

        // Add the test name to the PDF
        page.Canvas.DrawString("Test Name: " + test.Name, font, brush, 50, y);
        y += 20f;

        // Add the test description to the PDF
        page.Canvas.DrawString("Test Description: " + test.Description, font, brush, 50, y);
        y += 40f;

        // Loop through each question in the test
        for (var i = 0; i < test.Questions.Count; i++)
        {
            // Add the question text to the PDF
            page.Canvas.DrawString($"Question {i + 1}: {test.Questions[i].QuestionText}", font, brush, 50, y);
            y += 20f;

            // Loop through each answer in the question
            for (var j = 0; j < test.Questions[i].Answers.Count; j++)
            {
                // Add the answer text to the PDF
                page.Canvas.DrawString($"{(char)(97 + j) + ")"} {test.Questions[i].Answers[j].AnswerText}", font, brush, 70, y);

                // If the answer is correct, mark it as correct in the PDF
                if (test.Questions[i].Answers[j].IsCorrect)
                {
                    page.Canvas.DrawString("✔", font, brush, 50, y);
                }

                y += 20f;
            }

            // Add some space between questions
            y += 20f;
        }

        // Save the PDF to a memory stream
        var stream = new MemoryStream();
        pdfDoc.SaveToStream(stream);

        // Dispose of the PDF document
        pdfDoc.Dispose();

        // Reset the memory stream position to the beginning
        stream.Position = 0;

        return stream;
    }

    private string GetTextFromSavedFile(IFormFile file)
    {
        string cleanedText;

        try
        {
            if (!IsFileValid(file))
            {
                throw new Exception("File not valid!");
            }

            var doc = new Document();
            var filePath = GetPath(file);
            doc.LoadFromFile(filePath);

            var text = doc.GetText();

            cleanedText = text.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.\r\n", "");
        }
        catch (Exception ex)
        {
            throw new Exception($"Message {ex.Message}");
        }

        return cleanedText;
    }

    private static bool IsFileValid(IFormFile file)
    {
        var allowedExtensions = new[] { ".doc", ".docx" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            //TempData["Message"] = $"File with {fileExtension} extension not allowed! Please add a .doc or .docx file.";
            return false;
        }

        if (file.Length <= 0)
        {
            //TempData["Message"] = "File is empty";
            return false;
        }

        return true;
    }

    private static string GetPath(IFormFile file)
    {
        var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), UploadFolderName));

        return Path.Combine(path, file.FileName);
    }

    private static string GetPathWithRandomFileName(IFormFile file)
    {
        var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), UploadFolderName));

        return Path.Combine(path, newFileName);
    }
}