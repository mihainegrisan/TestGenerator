using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Services;

public interface IChatGptClient
{
    Task<string> SendChatMessage(string message);

    Task<string> SendMessage(string message, int maxChunkSize);

    Test GetTestFromInput(string input);
}