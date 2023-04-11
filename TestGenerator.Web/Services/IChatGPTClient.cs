namespace TestGenerator.Web.Services;

public interface IChatGPTClient
{
    Task<string> SendMessage(string message, int maxChunkSize);
}