using TestGenerator.Web.Services;

namespace TestGenerator.Tests;

public class ChatGptClientTests
{
  private IChatGptClient _chatGptClient;
  private string _apiAnswer;

  [SetUp]
  public void Setup()
  {
    _apiAnswer = """
            1. What is the purpose of the Factory Method pattern?\r\na) To define an interface for object creation but have the actual instantiation done by a subclass \r\nb) To create families of related or dependent objects without specifying their concrete classes \r\nc) To build complex objects using a step-by-step approach \r\nd) None of the above \r\nAnswer: a) To define an interface for object creation but have the actual instantiation done by a subclass \r\n\r\n2. How does the Abstract Factory Pattern enforce the constraint that a family of related objects are often designed to be used together?\r\na) By defining an interface for object creation and having the actual instantiation done by a subclass \r\nb) By defining an interface for creating families of related or dependent objects without specifying their concrete classes \r\nc) By using a step-by-step approach to build complex objects \r\nd) None of the above \r\nAnswer: b) By defining an interface for creating families of related or dependent objects without specifying their concrete classes \r\n\r\n3. What is the advantage of using the Builder pattern?\r\na) Decoupling the code \r\nb) Creating an abstraction to object creation \r\nc) Enforcing the constraint that a family of related objects are often designed to be used together \r\nd) All of the above \r\nAnswer: a) Decoupling the code
            """;
  }

  [Test]
  public void Test1()
  {
    _chatGptClient = new ChatGptClient(new SecretsManager());
    var response = _chatGptClient.GetTestFromInput(_apiAnswer);

    Assert.IsNotNull(response);
  }
}