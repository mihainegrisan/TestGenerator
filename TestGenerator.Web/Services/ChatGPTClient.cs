using OpenAI_API;
using System.Text.RegularExpressions;
using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Services;

public class ChatGptClient : IChatGptClient
{
    private readonly string _apiKey;

    public ChatGptClient(SecretsManager secretsManager)
    {
        _apiKey = secretsManager.GetApiKey();
    }

    public async Task<string> SendChatMessage(string message)
    {
        var openAi = new OpenAIAPI(_apiKey);

        var chat = openAi.Chat.CreateConversation();

        // give instruction as System
        chat.AppendSystemMessage("""
		You are a teacher who creates a multiple choice test with only one answer per question being the correct one. If the user tells you "Extract 3 questions and 4 possible answers with only one correct answer from the next paragraph: "Knowledge Assistant leverages machine learning (ML) advances to help you automatically generate questions and answers from any textual content in minutes.", you will say something like: 
		"1. What technology does Knowledge Assistant use to generate questions and answers?
		a) Robotics
		b) Blockchain
		c) Artificial intelligence
		d) Virtual reality
		Answer: c) Artificial intelligence
		2. How long does it take for Knowledge Assistant to generate questions and answers?
		a) Hours
		b) Days
		c) Minutes
		d) Seconds
		Answer: c) Minutes
		3. Can Knowledge Assistant generate questions and answers from any type of content?
		a) Yes, only textual content
		b) Yes, any type of content
		c) No, only video content
		d) No, only audio content
		Answer: b) Yes, any type of content".  
		You only ever respond with the questions, answers and the correct answer. You do not say anything else.
		""");

        // give a few examples as user and assistant
        //chat.AppendUserInput("Is this an animal? Cat");
        //chat.AppendExampleChatbotOutput("Yes");
        //chat.AppendUserInput("Is this an animal? House");
        //chat.AppendExampleChatbotOutput("No");

        chat.AppendUserInput(
            $"""Extract 3 questions and 4 possible answers with only one correct answer from the next paragraph: "{message}".""");

        var response = await chat.GetResponseFromChatbotAsync();
        Console.WriteLine(response);
        return response;
    }

    public Task<string> SendMessage(string message, int maxChunkSize)
    {
        throw new NotImplementedException();
    }

    public Test GetTestFromInput(string input)
    {
        Test test = new Test();

        // Get name and description
        string[] nameAndDesc = input.Substring(0, input.IndexOf("\\r\\n\\r\\n")).Split("\\r\\n");
        test.Name = nameAndDesc[0].Trim();
        test.Description = nameAndDesc[1].Trim();

        // Get questions and answers
        string[] questionStrings = input.Split(new string[] { "\\r\\n\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);

        test.Questions = new List<Question>();
        foreach (string questionString in questionStrings)
        {
            Question question = new Question();

            string questionText = questionString.Substring(questionString.IndexOf('.') + 2);
            int answerStartIndex = questionString.IndexOf("\\r\\na)") + 6;

            // Get the options
            List<string> options = new List<string>();
            int optionIndex = 0;
            while (true)
            {
                string optionString = (char)(97 + optionIndex) + ")";
                int optionStartIndex = questionString.IndexOf(optionString, answerStartIndex);
                if (optionStartIndex == -1)
                {
                    break;
                }
                options.Add(optionString);
                optionIndex++;
            }

            // Get answer text and set IsCorrect flag
            string[] answerParts = questionString.Substring(answerStartIndex).Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);
            string answerOption = answerParts[0];
            string answerText = string.Join("\\r\\n", answerParts.Skip(1)).Trim();
            int correctAnswerIndex = options.IndexOf(answerOption);
            question.Answers = new List<Answer>();
            for (int i = 0; i < options.Count; i++)
            {
                string optionText = questionString.Substring(questionString.IndexOf(options[i]) + options[i].Length).Trim();
                bool isCorrect = i == correctAnswerIndex;
                question.Answers.Add(new Answer { AnswerText = optionText, IsCorrect = isCorrect });
            }

            // Set question text and add to test
            question.QuestionText = questionText.Substring(0, questionText.IndexOf("\\r\\n")).Trim();
            test.Questions.Add(question);
        }

        test.NumberOfQuestions = test.Questions.Count;
        test.NumberOfAnswersPerQuestion = test.Questions[0].Answers.Count;

        return test;
    }

    //public async Task<string> SendMessage(string message, int maxChunkSize = 250)
    //{
    //  var openAi = new OpenAIAPI(_apiKey);
    //  var completions = await openAi.Chat.CreateChatCompletionAsync(message);

    //  var fullResponse = completions.Choices[0].Message.Content;

    //  var chunks = SplitIntoChunks(fullResponse, maxChunkSize);

    //  var tasks = new List<Task<string>>();

    //  foreach (var chunk in chunks)
    //  {
    //    tasks.Add(Task.Run(async () =>
    //    {
    //      var chunkCompletions = await openAi.Chat.CreateChatCompletionAsync(chunk);

    //      return chunkCompletions.Choices[0].Message.Content;
    //    }));
    //  }

    //  var results = await Task.WhenAll(tasks);

    //  return string.Join(" ", results);
    //}

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

        if (!string.IsNullOrWhiteSpace(currentChunk)) chunks.Add(currentChunk.Trim());

        return chunks;
    }
}