using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Services;

public interface IChatGptClient
{
    Task<string> SendChatMessage(Test test, string message);

    Task<string> SendMessage(string message, int maxChunkSize);

    Test UpdateTestWithQuestionsAndAnswersFromApiResponse(Test test, string responseMessage);
}