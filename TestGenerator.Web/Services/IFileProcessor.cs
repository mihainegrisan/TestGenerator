namespace TestGenerator.Web.Services;

public interface IFileProcessor
{
    string GetTextFromFile(IFormFile file);

    Task<bool> UploadFile(IFormFile file);
}

