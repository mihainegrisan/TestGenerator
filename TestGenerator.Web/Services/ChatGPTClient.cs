using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace TestGenerator.Web.Services;

public class ChatGptClient : IChatGptClient
{
    private readonly string _apiKey;

    public ChatGptClient(SecretsManager secretsManager)
    {
        _apiKey = secretsManager.GetApiKey();
    }

    public async Task<string> SendMessageNew(string message, int maxChunkSize = 250)
    {
        var openai = new OpenAIAPI(_apiKey);
        var completions = await openai.Chat.CreateChatCompletionAsync(message);

        var fullResponse = completions.Choices[0].Message.Content;

        var chunks = SplitIntoChunks(fullResponse, maxChunkSize);

        var tasks = new List<Task<string>>();

        foreach (var chunk in chunks)
        {
            tasks.Add(Task.Run(async () =>
            {
                var chunkCompletions = await openai.Chat.CreateChatCompletionAsync(chunk);

                return chunkCompletions.Choices[0].Message.Content;
            }));
        }

        var results = await Task.WhenAll(tasks);

        return string.Join(" ", results);
    }


    public async Task<string> SendMessage(string message, int maxChunkSize = 250)
    {
        var openai = new OpenAIAPI(_apiKey);
        var completions = await openai.Completions.CreateCompletionsAsync(new CompletionRequest
        {
            Prompt = message,
            Model = Model.ChatGPTTurbo,
            MaxTokens = 60,
            StopSequence = "\n"
        });

        var fullResponse = completions.Completions[0].Text;

        var chunks = SplitIntoChunks(fullResponse, maxChunkSize);

        var tasks = new List<Task<string>>();

        foreach (var chunk in chunks)
        {
            tasks.Add(Task.Run(async () =>
            {
                var chunkCompletions = await openai.Completions.CreateCompletionsAsync(new CompletionRequest
                {
                    Prompt = chunk,
                    Model = Model.ChatGPTTurbo,
                    MaxTokens = 60,
                    StopSequence = "\n"
                });

                return chunkCompletions.Completions[0].Text;
            }));
        }

        var results = await Task.WhenAll(tasks);

        return string.Join(" ", results);
    }

    private static List<string> SplitIntoChunks(string text, int maxChunkSize)
    {
        var words = text.Split(' ');

        var chunks = new List<string>();
        var currentChunk = "";

        foreach (var word in words)
        {
            if ((currentChunk + word).Length > maxChunkSize)
            {
                chunks.Add(currentChunk.Trim());
                currentChunk = "";
            }

            currentChunk += $"{word} ";
        }

        if (!string.IsNullOrWhiteSpace(currentChunk))
        {
            chunks.Add(currentChunk.Trim());
        }

        return chunks;
    }

}