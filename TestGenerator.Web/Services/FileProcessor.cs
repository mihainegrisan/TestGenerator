using Spire.Doc;

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

            using (var stream = file.OpenReadStream())
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