using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace TestGenerator.Web.Services;

public class ChatGPTClient : IChatGPTClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ChatGPTClient(SecretsManager secretsManager)
    {
        _httpClient = new HttpClient();
        _apiKey = secretsManager.GetApiKey();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> SendMessage(string message, int maxChunkSize = 250)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.openai.com/v1/engines/davinci-codex/completions"),
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                prompt = message,
                max_tokens = 60,
                n = 1,
                stop = "\n",
            }), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        dynamic data = JsonConvert.DeserializeObject(responseContent);

        var fullResponse = data.choices[0].text;

        var chunks = SplitIntoChunks(fullResponse, maxChunkSize);

        var tasks = chunks.Select((Func<string, Task<dynamic>>)(async chunk =>
        {
            var chunkRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/engines/davinci-codex/completions"),
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    prompt = chunk,
                    max_tokens = 60,
                    n = 1,
                    stop = "\n",
                }), Encoding.UTF8, "application/json")
            };

            var chunkResponse = await _httpClient.SendAsync(chunkRequest);
            chunkResponse.EnsureSuccessStatusCode();

            var chunkResponseContent = await chunkResponse.Content.ReadAsStringAsync();

            dynamic chunkData = JsonConvert.DeserializeObject(chunkResponseContent);

            return chunkData.choices[0].text;
        }));

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