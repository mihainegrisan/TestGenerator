using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Services;

public interface IFileProcessor
{
    Task<string> GetTextFromFileAsync(IFormFile file);

    string GetErrorMessageFromString(string input);

    MemoryStream GeneratePdf(Test test);

    MemoryStream GenerateWord(Test test);

    Task<bool> UploadFile(IFormFile file);
}