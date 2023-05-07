using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.DAL.Repositories;
using TestGenerator.Web.Utility;

namespace TestGenerator.Web.Controllers;

public class QuestionController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IQuestionRepository _questionRepository;
    private readonly ITestRepository _testRepository;
    private readonly INotyfService _notifyService;

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

        var questions = _questionRepository.GetQuestions(sortOrder, searchString);

        return View(await PaginatedList<Question>.CreateAsync(questions, pageNumber ?? 1, pageSize ?? 10));
    }

    public async Task<IActionResult> CreateManualTestBySelectingQuestions()
    {
        var questions = await _questionRepository.GetQuestionsAsync();

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

        var selectedQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(selectedQuestionIds);
        
        var currentUser = await _userManager.GetUserAsync(User);
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
            Author = currentUser,
            AuthorId = currentUser.Id,
        };

        await _testRepository.AddTestAsync(test);

        _notifyService.Success("Test Created!");

        return RedirectToAction(nameof(Index), nameof(Test));
    }

    [HttpGet]
    public async Task<IActionResult> CreateTestAutomatically()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestAutomatically(string name, string description, int numberOfQuestions)
    {
        var randomQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(numberOfQuestions);

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            throw new Exception("User not found.");
        }

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
            Author = currentUser,
            AuthorId = currentUser.Id,
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
        question.Author = currentUser;
        question.AuthorId = currentUser.Id;

        await _questionRepository.AddQuestionAsync(question);

        _notifyService.Success("Question Created!");

        return View(nameof(Details), question);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

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

        var existingQuestion = await _questionRepository.GetQuestionAsync(question.QuestionId);

        existingQuestion.QuestionText = question.QuestionText;
        existingQuestion.Answers = question.Answers;

        await _questionRepository.UpdateQuestionAsync(existingQuestion);

        _notifyService.Success("Question Updated!");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var question = await _questionRepository.GetQuestionAsync(id);

        return View(question);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

        await _questionRepository.DeleteQuestionAsync(question.QuestionId);

        _notifyService.Success("Question Deleted!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

        return View(question);
    }
}