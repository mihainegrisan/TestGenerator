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

    public Test ParseQuestions(string input)
    {
    // Initialize test object
    var test = new Test
    {
      Name = "Factory Method, Abstract Factory, and Builder Patterns Test",
      Description = "Test your knowledge on the Factory Method, Abstract Factory, and Builder patterns.",
      NumberOfQuestions = 3,
      NumberOfAnswersPerQuestion = 4
    };

    var questions = new List<Question>();

    // Split input string into individual questions
    var regex = new Regex(@"(\d+)\. ([\s\S]*?)(?=\d+\.|$)");
    var matches = regex.Matches(input);

    // Loop through each question and extract information
    foreach (Match match in matches)
    {
      var questionNumber = int.Parse(match.Groups[1].Value);
      var questionText = match.Groups[2].Value.Trim();
      var answerRegex = new Regex($@"([a-z])\) ([\s\S]*?)(?=\r\n[a-z]\)|Answer:|\z)");
      var answerMatches = answerRegex.Matches(match.Value);

      // Initialize question object
      var question = new Question
      {
        QuestionText = questionText,
        Answers = new List<Answer>()
      };

      // Loop through each answer and extract information
      foreach (Match answerMatch in answerMatches)
      {
        var answerLetter = answerMatch.Groups[1].Value;
        var answerText = answerMatch.Groups[2].Value.Trim();
        var isCorrect = answerMatch.Value.Contains("(Answer: ") && answerMatch.Value.Contains("a)");

        // Add answer to question object
        var answer = new Answer
        {
          AnswerText = answerText,
          IsCorrect = isCorrect
        };
        question.Answers.Add(answer);
      }

      // Add question to test object
      questions.Add(question);
    }

    // Add questions to test object
    test.Questions = questions;

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