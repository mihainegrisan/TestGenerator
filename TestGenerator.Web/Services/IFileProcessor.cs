namespace TestGenerator.Web.Services;

public interface IFileProcessor
{
    Task<string> GetTextFromFileAsync(IFormFile file);

    Task<bool> UploadFile(IFormFile file);
}

