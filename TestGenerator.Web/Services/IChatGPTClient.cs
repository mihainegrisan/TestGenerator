namespace TestGenerator.Web.Services;

public interface IChatGptClient
{
    Task<string> SendMessage(string message, int maxChunkSize);
}