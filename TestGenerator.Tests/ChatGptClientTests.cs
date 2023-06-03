using TestGenerator.DAL.Models;
using TestGenerator.Web.Services;

namespace TestGenerator.Tests;

public class ChatGptClientTests
{
    private string _apiAnswer;
    private IChatGptClient _chatGptClient;

    [SetUp]
    public void Setup()
    {
        var t = new Test
        {
            Name = "Test 1",
            Description = "Test 1 description",
            NumberOfQuestions = 3,
            NumberOfAnswersPerQuestion = 4,
            Questions = new List<Question>
            {
                new()
                {
                    QuestionText = "What is the Factory Method Pattern?",
                    Answers = new List<Answer>
                    {
                        new()
                        {
                            AnswerText = "A pattern that defines an interface for creating families of related or dependent objects without specifying their concrete classes",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "A pattern that creates complex objects by using a step-by-step approach",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "A pattern that defines an interface for object creation but the actual instantiation is done by a subclass",
                            IsCorrect = true
                        },
                        new()
                        {
                            AnswerText = "None of the above",
                            IsCorrect = false
                        }
                    }
                },
                new()
                {
                    QuestionText = "What is the purpose of the Abstract Factory Pattern?",
                    Answers = new List<Answer>
                    {
                        new()
                        {
                            AnswerText = "To create an abstraction to object creation",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "To build complex objects by using a step-by-step approach",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "To define an interface for object creation but the actual instantiation is done by a subclass",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "To provide an interface for creating families of related or dependent objects without specifying their concrete classes",
                            IsCorrect = true
                        }
                    }
                },
                new()
                {
                    QuestionText = "What is the purpose of the Abstract Factory Pattern?",
                    Answers = new List<Answer>
                    {
                        new()
                        {
                            AnswerText = "A pattern that defines an interface for creating families of related or dependent objects without specifying their concrete classes",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "A pattern that creates complex objects by using a step-by-step approach",
                            IsCorrect = true
                        },
                        new()
                        {
                            AnswerText = "A pattern that defines an interface for object creation but the actual instantiation is done by a subclass",
                            IsCorrect = false
                        },
                        new()
                        {
                            AnswerText = "None of the above",
                            IsCorrect = false
                        }
                    }
                }
            }
        };

        _apiAnswer =
            "1. What is the Factory Method Pattern?\r\na) A pattern that defines an interface for object creation and leaves the instantiation to a subclass\r\nb) A pattern that uses an interface for creating families of related objects without specifying their concrete classes\r\nc) A pattern that allows you to build complex objects by using a step-by-step approach\r\nd) None of the above\r\nAnswer: a) A pattern that defines an interface for object creation and leaves the instantiation to a subclass\r\n\r\n2. What is the purpose of the Abstract Factory Pattern?\r\na) To decouple code\r\nb) To build complex objects by using a step-by-step approach\r\nc) To enforce the use of related objects together\r\nd) None of the above\r\nAnswer: c) To enforce the use of related objects together\r\n\r\n3. How does the Builder pattern help control object creation?\r\na) By using an interface for object creation and leaving the instantiation to a subclass\r\nb) By using an interface for creating families of related objects without specifying their concrete classes\r\nc) By using a step-by-step approach\r\nd) None of the above\r\nAnswer: c) By using a step-by-step approach";
    }

    [Test]
    public void Test1()
    {
        _chatGptClient = new ChatGptClient(new SecretsManager());

        var initialTest = new Test
        {
            Name = "Test Name",
            Description = "Test Description",
            Questions = null,
            NumberOfQuestions = 3,
            NumberOfAnswersPerQuestion = 4
        };

        var test = _chatGptClient.PopulateTestWithApiResponse(initialTest, _apiAnswer);

        Assert.IsNotNull(test.Questions);
        Assert.IsNotNull(test.Questions[0].Answers);
        Assert.IsNotNull(test.Questions[1].Answers);
        Assert.IsNotNull(test.Questions[2].Answers);
    }
}