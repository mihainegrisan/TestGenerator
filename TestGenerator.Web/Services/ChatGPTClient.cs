using OpenAI_API;
using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Services;

public class ChatGptClient : IChatGptClient
{
    private readonly string _apiKey;

    public ChatGptClient(SecretsManager secretsManager)
    {
        _apiKey = secretsManager.GetApiKey();
    }

    public async Task<string> SendChatMessage(Test test, string message)
    {
        var openAi = new OpenAIAPI(_apiKey);

        var chat = openAi.Chat.CreateConversation();

        chat.AppendSystemMessage("""
		You are a teacher who creates a multiple choice test with only one answer per question being the correct one. For example, if the user tells you "Extract 3 questions and 4 possible answers with only one correct answer from the next paragraph: "Knowledge Assistant leverages machine learning (ML) advances to help you automatically generate questions and answers from any textual content in minutes.", you will say something like: 

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

		The Questions will always be separated by a new line. You only ever respond with the questions, answers and the correct answer. You do not say anything else. Also the number of questions and answers will differ from test to test. It's up to you to extract the questions and answers from the paragraph but you must return the exact number of questions and answers that the user asked for.
		""");

        chat.AppendUserInput(
            $"""Extract {test.NumberOfQuestions} questions and {test.NumberOfAnswersPerQuestion} answers per question with only one correct answer from the next paragraph: "{message}". You must respect the number of questions and answers per question requested.""");

        var response = await chat.GetResponseFromChatbotAsync();

        return response;
    }

    public Test PopulateTestWithApiResponse(Test test, string responseMessage)
    {
        var questionStrings = responseMessage.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        test.Questions = new List<Question>();

        foreach (var questionString in questionStrings)
        {
            var question = new Question();

            var questionText = GetQuestionText(questionString);

            var answerStartIndex = questionString.IndexOf("\r\na)", StringComparison.Ordinal) + 2;

            var options = GetOptions(questionString, answerStartIndex);

            // Get answer text and set IsCorrect flag
            var answerParts = questionString.Substring(answerStartIndex).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var startIndex = answerParts[options.Count].IndexOf(' ') + 1; // +1 to skip the space after "Answer:"
            var length = answerParts[options.Count].IndexOf(')') - startIndex + 1; // +1 to take the ")" after the letter "a)"
            var answerOption = answerParts[options.Count].Substring(startIndex, length).Trim();

            var correctAnswerIndex = options.IndexOf(answerOption);

            question.Answers = new List<Answer>();

            for (var i = 0; i < options.Count; i++)
            {
                var optionText = questionString.Substring(questionString.IndexOf(options[i], StringComparison.Ordinal) + options[i].Length).Trim();
                optionText = optionText.Substring(0, optionText.IndexOf("\r\n", StringComparison.Ordinal)).Trim();
                var isCorrect = i == correctAnswerIndex;
                question.Answers.Add(new Answer { AnswerText = optionText, IsCorrect = isCorrect });
            }

            question.QuestionText = questionText.Substring(0, questionText.IndexOf("\r\n", StringComparison.Ordinal)).Trim();

            test.Questions.Add(question);
        }

        test.NumberOfQuestions = test.Questions.Count;
        if (test.Questions.Count > 0)
        {
            test.NumberOfAnswersPerQuestion = test.Questions[0].Answers.Count;
        }

        return test;
    }

    public async Task<string> SendMessage(string message, int maxChunkSize = 250)
    {
        var openAi = new OpenAIAPI(_apiKey);
        var completions = await openAi.Chat.CreateChatCompletionAsync(message);

        var fullResponse = completions.Choices[0].Message.Content;

        var chunks = SplitIntoChunks(fullResponse, maxChunkSize);

        var tasks = new List<Task<string>>();

        foreach (var chunk in chunks)
        {
            tasks.Add(Task.Run(async () =>
            {
                var chunkCompletions = await openAi.Chat.CreateChatCompletionAsync(chunk);

                return chunkCompletions.Choices[0].Message.Content;
            }));
        }

        var results = await Task.WhenAll(tasks);

        return string.Join(" ", results);
    }

    private static string GetQuestionText(string questionString)
    {
        var questionNumberEndIndex = questionString.IndexOf('.') + 1;

        while (char.IsDigit(questionString[questionNumberEndIndex]))
        {
            questionNumberEndIndex++;
        }

        return questionString.Substring(questionNumberEndIndex + 1);
    }

    private static List<string> GetOptions(string questionString, int answerStartIndex)
    {
        var options = new List<string>();
        var optionIndex = 0;

        while (true)
        {
            var optionString = (char)(97 + optionIndex) + ")";
            var optionStartIndex = questionString.IndexOf(optionString, answerStartIndex, StringComparison.Ordinal);

            if (optionStartIndex == -1)
            {
                break;
            }

            options.Add(optionString);
            optionIndex++;
        }

        return options;
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