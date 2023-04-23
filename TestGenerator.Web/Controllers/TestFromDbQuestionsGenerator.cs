using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers;

public class TestFromDbQuestionsGenerator : Controller
{
    private readonly IFileProcessor _fileProcessor;
    private readonly IQuestionRepository _questionRepository;
    private readonly ITestRepository _testRepository;

    public TestFromDbQuestionsGenerator(ITestRepository testRepository, IQuestionRepository questionRepository, IFileProcessor fileProcessor)
    {
        _testRepository = testRepository;
        _questionRepository = questionRepository;
        _fileProcessor = fileProcessor;
    }

    public IActionResult Index()
    {
        return View();
    }

    //public async Task<IActionResult> GenerateTest()
    //{
    //    // Retrieve all the questions from the database
    //    List<Question> allQuestions = await _questionRepository.GetAllQuestionsAsync();

    //    // Select a subset of questions to include in the test (e.g. 10 questions)
    //    var numberOfQuestionsInTest = 10;
    //    var questionsForTest = allQuestions.OrderBy(x => Guid.NewGuid()).Take(numberOfQuestionsInTest).ToList();

    //    // Create a new test object
    //    var test = new Test
    //    {
    //        Questions = questionsForTest
    //    };

    //    // Save the test to the database
    //    await _testRepository.AddTestAsync(test);

    //    // Redirect to the test details page
    //    return RedirectToAction(nameof(TestController.Details), "Test", new { id = test.TestId });
    //}
}