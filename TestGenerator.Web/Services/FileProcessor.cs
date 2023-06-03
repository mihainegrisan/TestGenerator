using System.Drawing;
using System.Text.Json;
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
        var pdfDoc = new PdfDocument();
        var page = pdfDoc.Pages.Add();

        var brush = new PdfSolidBrush(Color.Black);
        var font = new PdfTrueTypeFont(new Font("Arial", 14f), true);

        // Set the starting y-coordinate for the text
        var y = 50f;

        // Set the text formatting options for wrapping
        var format = new PdfStringFormat
        {
            LineAlignment = PdfVerticalAlignment.Top,
            Alignment = PdfTextAlignment.Left,
            WordWrap = PdfWordWrapType.Word
        };

        // Add the test name to the PDF
        var testNameSize = font.MeasureString("Test Name: " + test.Name, page.GetClientSize().Width - 100, format);
        if (y + testNameSize.Height > page.GetClientSize().Height)
        {
            page = pdfDoc.Pages.Add();
            y = 50f;
        }
        page.Canvas.DrawString("Test Name: " + test.Name, font, brush, new RectangleF(50, y, page.GetClientSize().Width - 100, testNameSize.Height), format);
        y += testNameSize.Height + 20f;

        // Add the test description to the PDF
        var testDescriptionSize = font.MeasureString("Test Description: " + test.Description, page.GetClientSize().Width - 100, format);
        if (y + testDescriptionSize.Height > page.GetClientSize().Height)
        {
            page = pdfDoc.Pages.Add();
            y = 50f;
        }
        page.Canvas.DrawString("Test Description: " + test.Description, font, brush, new RectangleF(50, y, page.GetClientSize().Width - 100, testDescriptionSize.Height), format);
        y += testDescriptionSize.Height + 40f;

        // Loop through each question in the test
        for (var i = 0; i < test.Questions.Count; i++)
        {
            // Add the question text to the PDF
            var questionTextSize = font.MeasureString($"Question {i + 1}: {test.Questions[i].QuestionText}", page.GetClientSize().Width - 100, format);
            if (y + questionTextSize.Height > page.GetClientSize().Height)
            {
                page = pdfDoc.Pages.Add();
                y = 50f;
            }
            page.Canvas.DrawString($"Question {i + 1}: {test.Questions[i].QuestionText}", font, brush, new RectangleF(50, y, page.GetClientSize().Width - 100, questionTextSize.Height), format);
            y += questionTextSize.Height + 20f;

            // Loop through each answer in the question
            for (var j = 0; j < test.Questions[i].Answers.Count; j++)
            {
                // Add the answer text to the PDF
                var answerTextSize = font.MeasureString($"{(char)(97 + j) + ")"} {test.Questions[i].Answers[j].AnswerText}", page.GetClientSize().Width - 120, format);
                if (y + answerTextSize.Height > page.GetClientSize().Height)
                {
                    page = pdfDoc.Pages.Add();
                    y = 50f;
                }
                page.Canvas.DrawString($"{(char)(97 + j) + ")"} {test.Questions[i].Answers[j].AnswerText}", font, brush, new RectangleF(70, y, page.GetClientSize().Width - 120, answerTextSize.Height), format);

                // If the answer is correct, mark it as correct in the PDF
                if (test.Questions[i].Answers[j].IsCorrect)
                {
                    page.Canvas.DrawString("✔", font, brush, 50, y);
                }

                y += answerTextSize.Height + 20f;
            }

            // Add some space between questions
            y += 20f;
        }

        // Save the PDF to a memory stream
        var stream = new MemoryStream();
        pdfDoc.SaveToStream(stream);

        // Dispose of the PDF document
        pdfDoc.Close();

        // Reset the memory stream position to the beginning
        stream.Position = 0;

        return stream;
    }


    public string GetErrorMessageFromString(string input)
    {
        string message = string.Empty;

        int startIndex = input.IndexOf("\"message\": \"");
        if (startIndex != -1)
        {
            startIndex += "\"message\": \"".Length;
            int endIndex = input.IndexOf("\"", startIndex);
            if (endIndex != -1)
            {
                message = input.Substring(startIndex, endIndex - startIndex);
            }
        }

        return message;
    }

    public MemoryStream GenerateWord(Test test)
    {
        // Create a new Word document
        var doc = new Document();

        // Add a new section to the document
        var section = doc.AddSection();
        section.AddParagraph();

        // Create a new paragraph for the test name and add it to the section
        var testNamePara = section.AddParagraph();
        testNamePara.AppendText("Test Name: ");
        testNamePara.AppendText(test.Name);

        // Create a new paragraph for the test description and add it to the section
        var testDescPara = section.AddParagraph();
        testDescPara.AppendText("Test Description: ");
        testDescPara.AppendText(test.Description + "\n");

        // Loop through each question in the test
        for (var i = 0; i < test.Questions.Count; i++)
        {
            // Create a new paragraph for the question text and add it to the section
            var questionPara = section.AddParagraph();
            questionPara.AppendText($"{i + 1}. ");
            questionPara.AppendText(test.Questions[i].QuestionText);

            // Loop through each answer in the question
            for (var j = 0; j < test.Questions[i].Answers.Count; j++)
            {
                // Create a new paragraph for the answer text and add it to the section
                var answerPara = section.AddParagraph();
                answerPara.AppendText($"\t{(char)(97 + j)}) ");
                answerPara.AppendText(test.Questions[i].Answers[j].AnswerText);

                // If the answer is correct, mark it as correct in the paragraph
                //if (test.Questions[i].Answers[j].IsCorrect)
                //{
                //  answerPara.AppendText(" ✔");
                //}
            }

            // Add some space between questions
            section.AddParagraph();
        }

        // Save the Word document to a memory stream
        var stream = new MemoryStream();
        doc.SaveToStream(stream, FileFormat.Docx);

        // Dispose of the Word document
        doc.Dispose();

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