using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.DAL.Repositories;
using TestGenerator.Web.Services;
using TestGenerator.Web.Utility;

namespace TestGenerator.Web.Controllers;

public class TestGeneratorController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IChatGptClient _chatGptClient;
    private readonly IFileProcessor _fileProcessor;
    private readonly ITestRepository _testRepository;
    private readonly INotyfService _notifyService;

    public TestGeneratorController(
        UserManager<IdentityUser> userManager,
        ITestRepository testRepository,
        IFileProcessor fileProcessor,
        IChatGptClient chatGptClient,
        INotyfService notifyService)
    {
        _userManager = userManager;
        _testRepository = testRepository;
        _fileProcessor = fileProcessor;
        _chatGptClient = chatGptClient;
        _notifyService = notifyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? sortOrder,
        string? currentFilter,
        string? searchString,
        int? pageNumber,
        int? pageSize)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
        ViewData["PageSize"] = pageSize;

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var tests = _testRepository.GetTests(sortOrder, searchString, true);

        return View(await PaginatedList<Test>.CreateAsync(tests, pageNumber ?? 1, pageSize ?? 10));
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
  //  if (!ModelState.IsValid)
  //  {
  //    return View(test);
  //  }

  //  var chatMessage = await _fileProcessor.GetTextFromFileAsync(file);

  //  _notifyService.Information("Text extracted from file!");

  //  var responseMessage = await _chatGptClient.SendChatMessage(test, chatMessage);

  //  _notifyService.Information("Received response from ChatGPT!");

  //  _chatGptClient.PopulateTestWithApiResponse(test, responseMessage);

  //  _notifyService.Information("Populated test with ChatGPT response!");

  //  TempData["Test"] = test;

  //  return View(nameof(GenerateTest), test);
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
            IsCreatedManually = false,
            IsAutoCreatedFromQuestions = false,
            IsAutoCreatedByChatGpt = true,
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

        test.IsCreatedManually = false;
        test.IsAutoCreatedFromQuestions = false;
        test.IsAutoCreatedByChatGpt = true;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            test.Author = currentUser;
            test.AuthorId = currentUser.Id;
            test.CreatedAt = DateTime.Now;
        }
        else
        {
            throw new Exception("User not found.");
        }

        foreach (var question in test.Questions)
        {
            question.CreatedAt = DateTime.Now;
            question.Author = currentUser;
            question.AuthorId = currentUser.Id;
        }

        await _testRepository.AddTestAsync(test);

        _notifyService.Success("Test Created!");

        return View(nameof(Details), test);
    }

    public async Task<IActionResult> DownloadPdf(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        var stream = _fileProcessor.GeneratePdf(test);

        stream.Position = 0;

        return File(stream, "application/pdf", $"{test.Name}.pdf");
    }

    public async Task<IActionResult> DownloadWord(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        var stream = _fileProcessor.GenerateWord(test);

        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{test.Name}.docx");
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
        existingTest.Questions = test.Questions;

        await _testRepository.UpdateTestAsync(existingTest);

        _notifyService.Success("Test Updated!");

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

        _notifyService.Success("Test Deleted!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }
}