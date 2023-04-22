using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers;

public class TestGeneratorController : Controller
{
    private readonly IChatGptClient _chatGptClient;
    private readonly IFileProcessor _fileProcessor;
    private readonly ITestRepository _testRepository;

    public TestGeneratorController(ITestRepository testRepository, IFileProcessor fileProcessor,
        IChatGptClient chatGptClient)
    {
        _testRepository = testRepository;
        _fileProcessor = fileProcessor;
        _chatGptClient = chatGptClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tests = await _testRepository.GetTestsAsync();
        return View(tests);
    }

    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        return View();
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Generate(Test test, IFormFile file)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(test);
    //    }

    //    var chatMessage = await _fileProcessor.GetTextFromFileAsync(file);

    //    var responseMessage = await _chatGptClient.SendChatMessage(test, chatMessage);

    //    _chatGptClient.PopulateTestWithApiResponse(test, responseMessage);

    //    TempData["Test"] = test;

    //    return View(nameof(GenerateTest), test);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(Test test, IFormFile file)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        var chatMessage = await _fileProcessor.GetTextFromFileAsync(file);


        var responseMessage =
            "1. What is the Factory Method Pattern?\r\na) A pattern that defines an interface for object creation and leaves the instantiation to a subclass\r\nb) A pattern that uses an interface for creating families of related objects without specifying their concrete classes\r\nc) A pattern that allows you to build complex objects by using a step-by-step approach\r\nd) None of the above\r\nAnswer: a) A pattern that defines an interface for object creation and leaves the instantiation to a subclass\r\n\r\n2. What is the purpose of the Abstract Factory Pattern?\r\na) To decouple code\r\nb) To build complex objects by using a step-by-step approach\r\nc) To enforce the use of related objects together\r\nd) None of the above\r\nAnswer: c) To enforce the use of related objects together\r\n\r\n3. How does the Builder pattern help control object creation?\r\na) By using an interface for object creation and leaving the instantiation to a subclass\r\nb) By using an interface for creating families of related objects without specifying their concrete classes\r\nc) By using a step-by-step approach\r\nd) None of the above\r\nAnswer: c) By using a step-by-step approach";

        test = new Test
        {
            Name = test.Name,
            Description = test.Description,
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

        TempData["Test"] = test;

        _chatGptClient.PopulateTestWithApiResponse(test, responseMessage);

        return View(nameof(GenerateTest), test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateTest(Test test)
    {
        //bool isUploaded = await _fileProcessor.UploadFile(file);

        //if (!isUploaded)
        //{
        //  return View();
        //}

        //TempData["Message"] = "File uploaded successfully";
        // From now on, work with the saved file

        if (!ModelState.IsValid)
        {
            return View(test);
        }

        await _testRepository.AddTestAsync(test);

        var tests = await _testRepository.GetTestsAsync();

        return View(nameof(Index), tests);
    }

    public async Task<IActionResult> DownloadPdf(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        var stream = _fileProcessor.GeneratePdf(test);
        stream.Position = 0;

        return File(stream, "application/pdf", $"{test.Name}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Test test)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        var existingTest = await _testRepository.GetTestAsync(test.TestId);

        existingTest.Name = test.Name;
        existingTest.Description = test.Description;
        existingTest.NumberOfQuestions = test.NumberOfQuestions;
        existingTest.NumberOfAnswersPerQuestion = test.NumberOfAnswersPerQuestion;

        await _testRepository.UpdateTestAsync(existingTest);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        await _testRepository.DeleteTestAsync(test.TestId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }
}