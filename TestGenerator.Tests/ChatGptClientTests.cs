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

        var newest =
            "1. What is the purpose of the Factory Method pattern?\r\na) To create an interface for object creation\r\nb) To define the actual instantiation of a subclass\r\nc) To enforce a constraint between related objects\r\nd) To build complex objects using a step-by-step approach\r\nAnswer: a) To create an interface for object creation\r\n\r\n2. What is the advantage of decoupling the code with the Abstract Factory Pattern?\r\na) It allows for creating families of related objects without specifying their concrete classes\r\nb) It enforces a constraint between related objects\r\nc) It allows for complex objects to be built using a step-by-step approach\r\nd) It defines the actual instantiation of a subclass\r\nAnswer: a) It allows for creating families of related objects without specifying their concrete classes\r\n\r\n3. What is the aim of the Builder pattern?\r\na) To create an abstraction to object creation\r\nb) To use an interface for creating families of related objects\r\nc) To build complex objects using a step-by-step approach\r\nd) To enforce a constraint between related objects\r\nAnswer: c) To build complex objects using a step-by-step approach";

        var resp =
            "1. What is the Factory method pattern?\r\na) A pattern where we define an interface and its implementation in a subclass\r\nb) A pattern where we define an interface for object creation, but object instantiation is done by a subclass\r\nc) A pattern where abstract factories are used for creating families of related or dependent objects\r\nd) A pattern where complex objects are built using a step-by-step approach\r\nAnswer: b) A pattern where we define an interface for object creation, but object instantiation is done by a subclass\r\n\r\n2. What is the main advantage of the factory method pattern?\r\na) That it allows you to build complex objects by using a step-by-step approach\r\nb) That it decouples the code\r\nc) That it enforces the constraint of using related objects together\r\nd) That it allows you to create families of related or dependent objects\r\nAnswer: b) That it decouples the code\r\n\r\n3. What is the Abstract Factory pattern?\r\na) A pattern where we define an interface for object creation, but the actual instantiation is done by a subclass\r\nb) A pattern where we use an interface for creating families of related or dependent objects without specifying their concrete classes\r\nc) A pattern where we build complex objects by using a step-by-step approach\r\nd) None of the above\r\nAnswer: b) A pattern where we use an interface for creating families of related or dependent objects without specifying their concrete classes\r\n\r\n4. How are the methods of an Abstract Factory pattern implemented?\r\na) As builder methods\r\nb) As step-by-step methods\r\nc) As factory methods\r\nd) As subclass methods\r\nAnswer: c) As factory methods\r\n\r\n5. What is the purpose of passing an instance of the Abstract Factory pattern into some code?\r\na) To control how the object is created\r\nb) To decouple the clients from the actual concrete classes they use\r\nc) To ensure that related objects are used together\r\nd) To specify the concrete classes of related objects\r\nAnswer: b) To decouple the clients from the actual concrete classes they use\r\n\r\n6. What is the Builder pattern?\r\na) A pattern where we define an interface for object creation, but the actual instantiation is done by a subclass\r\nb) A pattern where we use an interface for creating families of related or dependent objects without specifying their concrete classes\r\nc) A pattern where we build complex objects by using a step-by-step approach\r\nd) None of the above\r\nAnswer: c) A pattern where we build complex objects by using a step-by-step approach\".";
    }

    [Test]
    public void Test1()
    {
        _chatGptClient = new ChatGptClient(new SecretsManager());

        var test = new Test
        {
            Name = "Test Name",
            Description = "Test Description",
            Questions = null,
            NumberOfQuestions = 3,
            NumberOfAnswersPerQuestion = 4
        };

        var response = _chatGptClient.PopulateTestWithApiResponse(test, _apiAnswer);

        Assert.IsNotNull(test.Questions);
        Assert.IsNotNull(test.Questions[0].Answers);
        Assert.IsNotNull(test.Questions[1].Answers);
        Assert.IsNotNull(test.Questions[2].Answers);
    }
}