using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.DAL.Repositories;
using TestGenerator.Web.Utility;

namespace TestGenerator.Web.Controllers;

[Authorize]
public class QuestionController : Controller
{
    private readonly INotyfService _notifyService;
    private readonly IQuestionRepository _questionRepository;
    private readonly ITestRepository _testRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public QuestionController(
        UserManager<IdentityUser> userManager,
        IQuestionRepository questionRepository,
        ITestRepository testRepository,
        INotyfService notifyService)
    {
        _userManager = userManager;
        _questionRepository = questionRepository;
        _testRepository = testRepository;
        _notifyService = notifyService;
    }

    public async Task<IActionResult> Index(
        string? sortOrder,
        string? currentFilter,
        string? searchString,
        int? pageNumber,
        int? pageSize)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["QuestionNameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "question_name_desc" : "";
        ViewData["TestNameSortParam"] = sortOrder == "test_name" ? "test_name_desc" : "test_name";
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

        var currentUser = await _userManager.GetUserAsync(User);

        var questionViewModels = _questionRepository.GetQuestions(sortOrder, searchString, currentUser.Id);

        return View(await PaginatedList<QuestionTestViewModel>.CreateAsync(questionViewModels, pageNumber ?? 1, pageSize ?? 10));
    }

    public async Task<IActionResult> CreateManualTestBySelectingQuestions()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var questions = await _questionRepository.GetQuestionsAsync(currentUser.Id);

        return View(questions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateManualTestBySelectingQuestions(List<int>? selectedQuestionIds, string name, string description)
    {
        if (selectedQuestionIds?.Count == 0)
        {
            TempData["ErrorMessage"] = "Please select at least one question.";

            return RedirectToAction(nameof(CreateManualTestBySelectingQuestions));
        }

        var currentUser = await _userManager.GetUserAsync(User);

        var selectedQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(selectedQuestionIds, currentUser.Id);

        if (currentUser == null)
        {
            throw new Exception("User not found.");
        }

        var test = new Test
        {
            Name = name,
            Description = description,
            Questions = selectedQuestions,
            NumberOfQuestions = selectedQuestions.Count,
            NumberOfAnswersPerQuestion = selectedQuestions.Max(q => q.Answers.Count),
            IsCreatedManually = false,
            IsAutoCreatedFromQuestions = true,
            IsAutoCreatedByChatGpt = false,
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            Author = currentUser,
            AuthorId = currentUser.Id
        };

        await _testRepository.AddTestAsync(test);

        _notifyService.Success("Test Created!");

        return RedirectToAction(nameof(Index), nameof(Test));
    }

    [HttpGet]
    public async Task<IActionResult> CreateTestAutomatically()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var totalQuestionsInDatabase = await _questionRepository.GetNumberOfQuestionsInTheDatabase(currentUser.Id);

        ViewBag.TotalQuestionsInDatabase = totalQuestionsInDatabase;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestAutomatically(string name, string description, int numberOfQuestions)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var totalQuestionsInDatabase = await _questionRepository.GetNumberOfQuestionsInTheDatabase(currentUser.Id);

        if (totalQuestionsInDatabase < numberOfQuestions)
        {
            TempData["ErrorMessage"] = $"You don't have {numberOfQuestions} questions in the database!";

            ViewBag.TotalQuestionsInDatabase = totalQuestionsInDatabase;

            return View();
        }

        var randomQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(numberOfQuestions, currentUser.Id);

        var test = new Test
        {
            Name = name,
            Description = description,
            Questions = randomQuestions,
            NumberOfQuestions = randomQuestions.Count,
            NumberOfAnswersPerQuestion = randomQuestions.Max(q => q.Answers.Count),
            IsCreatedManually = false,
            IsAutoCreatedFromQuestions = true,
            IsAutoCreatedByChatGpt = false,
            CreatedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            Author = currentUser,
            AuthorId = currentUser.Id
        };

        await _testRepository.AddTestAsync(test);

        _notifyService.Success("Test Created!");

        return RedirectToAction(nameof(Index), nameof(Test));
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Question question, int numberOfAnswers)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        if (numberOfAnswers < 2)
        {
            TempData["ErrorMessage"] = "The number of answers per question should be higher than 1.";

            return View(question);
        }

        ViewBag.NumberOfAnswers = numberOfAnswers;

        return View(nameof(AddQuestion), question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion(Question question)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        if (!question.Answers.Any(a => a.IsCorrect))
        {
            TempData["ErrorMessage"] = "Please select at least one correct answer.";

            ViewBag.NumberOfAnswers = question.Answers.Count;

            return View(nameof(AddQuestion), question);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            throw new Exception("User not found.");
        }

        question.CreatedAt = DateTime.Now;
        question.EditedAt = DateTime.Now;
        question.Author = currentUser;
        question.AuthorId = currentUser.Id;

        await _questionRepository.AddQuestionAsync(question);

        _notifyService.Success("Question Created!");

        return View(nameof(Details), question);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var question = await _questionRepository.GetQuestionAsync(id, currentUser.Id);

        return View(question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Question question)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            question.Author = currentUser;
            question.AuthorId = currentUser.Id;
        }

        await _questionRepository.UpdateQuestionAsync(question, currentUser.Id);

        _notifyService.Success("Question Updated!");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        var question = await _questionRepository.GetQuestionAsync(id, currentUser.Id);

        return View(question);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var question = await _questionRepository.GetQuestionAsync(id, currentUser.Id);

        await _questionRepository.DeleteQuestionAsync(question.QuestionId, currentUser.Id);

        _notifyService.Success("Question Deleted!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var question = await _questionRepository.GetQuestionAsync(id, currentUser.Id);

        return View(question);
    }
}